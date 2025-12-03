using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Api.Features.Todos.Model;
using Api.Shared.Features.Api;
using IntegrationTests.Extensions;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.Tests.Todos;

public class AddAttachmentTests(TestingFixture fixture) : TestingBase(fixture)
{
    private static string GetRoute(Guid todoId)
    {
        return Routes.Todos.AddAttachment
            .Replace("{todoId:guid}", todoId.ToString());
    }

    private static MultipartFormDataContent CreateMultipartContent(
        string fieldName,
        string fileName,
        string contentType,
        int sizeInBytes = 128)
    {
        var content = new MultipartFormDataContent();

        var bytes = Encoding.UTF8.GetBytes(new string('x', sizeInBytes));
        var fileContent = new ByteArrayContent(bytes);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);

        // Important: name must match "Body.File" for model binding
        content.Add(fileContent, fieldName, fileName);

        return content;
    }

    [Fact]
    public async Task AddAttachment_WhenTodoExistsAndBelongsToUser_AddsAttachmentAndReturnsOk()
    {
        var client = CreateAuthenticatedClient();
        var userId = client.GetUserId();

        var todo = Todo.TryCreate("Test todo", "Desc", TodoPriority.Medium, userId).Value;
        DbContext.Todos.Add(todo);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var url = GetRoute(todo.Id.Value);

        var multipart = CreateMultipartContent(
            fieldName: "Body.File",
            fileName: "test.pdf",
            contentType: "application/pdf",
            sizeInBytes: 1024);

        var response = await client.PostAsync(url, multipart, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var updated = await DbContext.Todos
            .Include(t => t.Attachments)
            .AsNoTracking()
            .SingleAsync(t => t.Id == todo.Id, CurrentCancellationToken);

        Assert.Single(updated.Attachments);
        var attachment = updated.Attachments.First();
        Assert.Equal("test.pdf", attachment.FileName);
        Assert.Equal("application/pdf", attachment.ContentType);
        Assert.Equal(todo.Id, attachment.TodoId);
    }

    [Fact]
    public async Task AddAttachment_WhenTodoDoesNotExist_ReturnsNotFound()
    {
        var client = CreateAuthenticatedClient();

        var url = GetRoute(Guid.NewGuid());

        var multipart = CreateMultipartContent(
            fieldName: "Body.File",
            fileName: "test.pdf",
            contentType: "application/pdf");

        var response = await client.PostAsync(url, multipart, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AddAttachment_WhenFileTooLarge_ReturnsServerError()
    {
        var client = CreateAuthenticatedClient();
        var userId = client.GetUserId();

        var todo = Todo.TryCreate("Big file todo", "Desc", TodoPriority.Medium, userId).Value;
        DbContext.Todos.Add(todo);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var url = GetRoute(todo.Id.Value);

        // Just over 10 MB
        const int tooLargeBytes = 10 * 1024 * 1024 + 1;

        var multipart = CreateMultipartContent(
            fieldName: "Body.File",
            fileName: "huge.bin",
            contentType: "application/octet-stream",
            sizeInBytes: tooLargeBytes);

        var response = await client.PostAsync(url, multipart, CurrentCancellationToken);

        // With current TransformResult: ErrorType.Conflict -> StatusCode(500)
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task AddAttachment_WhenUserIsNotAuthenticated_ReturnsUnauthorized()
    {
        var client = CreateUnauthenticatedClient();

        var url = GetRoute(Guid.NewGuid());

        var multipart = CreateMultipartContent(
            fieldName: "Body.File",
            fileName: "test.pdf",
            contentType: "application/pdf");

        var response = await client.PostAsync(url, multipart, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
