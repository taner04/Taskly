using System.Net;
using System.Net.Http.Json;
using Api.Features.Attachments.Endpoints;
using Api.Features.Attachments.Models;
using Api.Features.Todos.Model;
using Api.Shared.Api;
using IntegrationTests.Extensions;

namespace IntegrationTests.Tests.Attachments;

public class DownloadAttachmentTests(TestingFixture fixture) : TestingBase(fixture)
{
    private static string GetRoute(
        Guid attachmentId)
    {
        return Routes.Attachments.Download.Replace("{attachmentId:guid}", attachmentId.ToString());
    }

    [Fact]
    public async Task DownloadAttachment_WhenExistsAndBelongsToUser_ReturnsOkWithSas()
    {
        var client = CreateAuthenticatedClient();
        var userId = client.GetUserId();

        var todo = new Todo("Task", "Desc", TodoPriority.Medium, userId);
        var attachment = Attachment.CreatePending(todo.Id, "file.txt", "text/plain");
        attachment.MarkUploaded(123);

        DbContext.Todos.Add(todo);
        DbContext.Attachments.Add(attachment);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var url = GetRoute(attachment.Id.Value);

        var response = await client.GetAsync(url, CurrentCancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var dto = await response.Content.ReadFromJsonAsync<DownloadAttachment.Dto>(CurrentCancellationToken);
        Assert.NotNull(dto);

        Assert.Contains("sig=", dto.DownloadUrl); // SAS signature present
        Assert.Equal("file.txt", dto.FileName);
        Assert.Equal("text/plain", dto.ContentType);
    }

    [Fact]
    public async Task DownloadAttachment_WhenNotFound_ReturnsBadRequest()
    {
        var client = CreateAuthenticatedClient();

        var url = GetRoute(Guid.NewGuid());
        var response = await client.GetAsync(url, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problem = await response.ReadProblemAsync();
        Assert.Equal("Attachment.NotFound", problem.ErrorCode);
    }

    [Fact]
    public async Task DownloadAttachment_WhenBelongsToAnotherUser_ReturnsNotFound()
    {
        var client = CreateAuthenticatedClient();

        var todo = new Todo("Other User Todo", "XXX", TodoPriority.Low, "auth0|other");
        var attachment = Attachment.CreatePending(todo.Id, "secure.txt", "text/plain");
        attachment.MarkUploaded(100);

        DbContext.Todos.Add(todo);
        DbContext.Attachments.Add(attachment);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var url = GetRoute(attachment.Id.Value);
        var response = await client.GetAsync(url, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problem = await response.ReadProblemAsync();
        Assert.Equal("Attachment.NotFound", problem.ErrorCode);
    }

    [Fact]
    public async Task DownloadAttachment_WhenUserNotAuthenticated_ReturnsUnauthorized()
    {
        var client = CreateUnauthenticatedClient();

        var url = GetRoute(Guid.NewGuid());
        var response = await client.GetAsync(url, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}