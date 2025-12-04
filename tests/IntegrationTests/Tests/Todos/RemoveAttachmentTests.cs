using System.Net;
using Api.Features.Attachments.Models;
using Api.Features.Todos.Model;
using Api.Shared.Api;
using IntegrationTests.Extensions;

namespace IntegrationTests.Tests.Todos;

public class RemoveAttachmentTests(TestingFixture fixture) : TestingBase(fixture)
{
    private static string GetRoute(
        Guid todoId,
        Guid attachmentId)
    {
        return Routes.Todos.RemoveAttachment
            .Replace("{todoId}", todoId.ToString())
            .Replace("{attachmentId}", attachmentId.ToString());
    }

    [Fact]
    public async Task RemoveAttachment_WhenExists_RemovesAttachmentAndDeletesBlob()
    {
        var client = CreateAuthenticatedClient();
        var userId = client.GetUserId();

        var todo = new Todo("Task", "Desc", TodoPriority.Low, userId);

        var attachment = Attachment.CreatePending(todo.Id, "file.txt", "text/plain");
        attachment.MarkUploaded(50);

        todo.Attachments.Add(attachment);

        DbContext.Todos.Add(todo);
        DbContext.Attachments.Add(attachment);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var url = GetRoute(todo.Id.Value, attachment.Id.Value);

        var response = await client.DeleteAsync(url, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var updated = await DbContext.Todos
            .Include(t => t.Attachments)
            .AsNoTracking()
            .SingleAsync(t => t.Id == todo.Id, CurrentCancellationToken);

        Assert.Empty(updated.Attachments);
    }

    [Fact]
    public async Task RemoveAttachment_WhenBlobDeleteFails_ReturnsInternalServerError()
    {
        var client = CreateAuthenticatedClient();
        var userId = client.GetUserId();

        var todo = new Todo("X", "YYY", TodoPriority.Medium, userId);
        var attachment = Attachment.CreatePending(todo.Id, "fail.txt", "text/plain");
        attachment.MarkUploaded(12);

        todo.Attachments.Add(attachment);

        DbContext.Todos.Add(todo);
        DbContext.Attachments.Add(attachment);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var url = GetRoute(todo.Id.Value, attachment.Id.Value);

        var response = await client.DeleteAsync(url, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
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

        var todo = new Todo("Task1", "Desc", TodoPriority.Low, userId);

        DbContext.Todos.Add(todo);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var url = GetRoute(todo.Id.Value, Guid.NewGuid());

        var response = await client.DeleteAsync(url, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task RemoveAttachment_WhenTodoBelongsToAnotherUser_ReturnsNotFound()
    {
        var client = CreateAuthenticatedClient();

        var todo = new Todo("Foreign", "Desc", TodoPriority.Low, "auth0|other");

        var attachment = Attachment.CreatePending(todo.Id, "file.png", "image/png");
        attachment.MarkUploaded(100);

        todo.Attachments.Add(attachment);

        DbContext.Todos.Add(todo);
        DbContext.Attachments.Add(attachment);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var url = GetRoute(todo.Id.Value, attachment.Id.Value);

        var response = await client.DeleteAsync(url, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task RemoveAttachment_WhenUserNotAuthenticated_ReturnsUnauthorized()
    {
        var client = CreateUnauthenticatedClient();

        var url = GetRoute(Guid.NewGuid(), Guid.NewGuid());

        var response = await client.DeleteAsync(url, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}