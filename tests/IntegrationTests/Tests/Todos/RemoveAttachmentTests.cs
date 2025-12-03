using System.Net;
using Api.Features.Attachments.Models;
using Api.Features.Todos.Model;
using Api.Shared.Features.Api;
using IntegrationTests.Extensions;
using Microsoft.AspNetCore.Http;

namespace IntegrationTests.Tests.Todos;

public class RemoveAttachmentTests(TestingFixture fixture) : TestingBase(fixture)
{
    private static string GetRoute(Guid todoId, Guid attachmentId)
    {
        return Routes.Todos.RemoveAttachment
            .Replace("{todoId:guid}", todoId.ToString())
            .Replace("{attachmentId:guid}", attachmentId.ToString());
    }

    [Fact]
    public async Task RemoveAttachment_WhenAttachmentExistsOnTodo_RemovesAttachmentAndReturnsOk()
    {
        var client = CreateAuthenticatedClient();
        var userId = client.GetUserId();

        var todo = Todo.TryCreate("Test todo", "Desc", TodoPriority.Medium, userId).Value;

        // Create a fake attachment for this todo
        var attachment = Attachment.TryCreate(
            todo.Id,
            new FormFile(
                baseStream: new MemoryStream(new byte[16]),
                baseStreamOffset: 0,
                length: 16,
                name: "file",
                fileName: "test.pdf"
            )
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/pdf"
            }).Value;

        todo.Attachments.Add(attachment);

        DbContext.Todos.Add(todo);
        DbContext.Attachments.Add(attachment);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var url = GetRoute(todo.Id.Value, attachment.Id.Value);

        var response = await client.DeleteAsync(url, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var updated = await DbContext.Todos
            .Include(t => t.Attachments)
            .AsNoTracking()
            .SingleAsync(t => t.Id == todo.Id, CurrentCancellationToken);

        Assert.DoesNotContain(updated.Attachments, a => a.Id == attachment.Id);
    }

    [Fact]
    public async Task RemoveAttachment_WhenTodoNotFound_ReturnsNotFound()
    {
        var client = CreateAuthenticatedClient();

        var url = GetRoute(Guid.NewGuid(), Guid.NewGuid());

        var response = await client.DeleteAsync(url, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task RemoveAttachment_WhenAttachmentNotOnTodo_ReturnsNotFound()
    {
        var client = CreateAuthenticatedClient();
        var userId = client.GetUserId();

        var todo = Todo.TryCreate("Todo with no attachments", "Desc", TodoPriority.Medium, userId).Value;

        DbContext.Todos.Add(todo);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var url = GetRoute(todo.Id.Value, Guid.NewGuid()); // random attachment id

        var response = await client.DeleteAsync(url, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task RemoveAttachment_WhenTodoBelongsToAnotherUser_ReturnsNotFound()
    {
        var client = CreateAuthenticatedClient();

        var todo = Todo.TryCreate("Foreign todo", "Desc", TodoPriority.Medium, "auth0|other-user").Value;

        var attachment = Attachment.TryCreate(
            todo.Id,
            new FormFile(
                baseStream: new MemoryStream(new byte[16]),
                baseStreamOffset: 0,
                length: 16,
                name: "file",
                fileName: "test.pdf"
            )
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/pdf"
            }).Value;

        todo.Attachments.Add(attachment);

        DbContext.Todos.Add(todo);
        DbContext.Attachments.Add(attachment);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var url = GetRoute(todo.Id.Value, attachment.Id.Value);

        var response = await client.DeleteAsync(url, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task RemoveAttachment_WhenAttachmentBelongsToAnotherTodo_ReturnsNotFound()
    {
        var client = CreateAuthenticatedClient();
        var userId = client.GetUserId();

        var todo1 = Todo.TryCreate("Todo 1", "Desc", TodoPriority.Medium, userId).Value;
        var todo2 = Todo.TryCreate("Todo 2", "Desc", TodoPriority.Medium, userId).Value;

        var attachment = Attachment.TryCreate(
            todo2.Id,
            new FormFile(
                baseStream: new MemoryStream(new byte[16]),
                baseStreamOffset: 0,
                length: 16,
                name: "file",
                fileName: "other.pdf"
            )
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/pdf"
            }).Value;

        todo2.Attachments.Add(attachment);

        DbContext.Todos.AddRange(todo1, todo2);
        DbContext.Attachments.Add(attachment);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        // Try to remove attachment from todo1, but it's attached to todo2
        var url = GetRoute(todo1.Id.Value, attachment.Id.Value);

        var response = await client.DeleteAsync(url, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task RemoveAttachment_WhenUserIsNotAuthenticated_ReturnsUnauthorized()
    {
        var client = CreateUnauthenticatedClient();

        var url = GetRoute(Guid.NewGuid(), Guid.NewGuid());

        var response = await client.DeleteAsync(url, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
