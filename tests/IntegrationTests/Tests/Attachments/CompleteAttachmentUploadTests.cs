using System.Net;
using System.Net.Http.Json;
using Api.Features.Attachments.Models;
using Api.Features.Todos.Model;
using Api.Shared.Api;
using IntegrationTests.Extensions;

namespace IntegrationTests.Tests.Attachments;

public class CompleteAttachmentUploadTests(TestingFixture fixture) : TestingBase(fixture)
{
    public static TheoryData<long> InvalidFileSizes => new()
    {
        0L,
        -1L,
        Attachment.MaxFileSizeInBytes + 1
    };

    private static string GetRoute(
        Guid attachmentId)
    {
        return Routes.Attachments.CompleteUpload.Replace("{attachmentId:guid}", attachmentId.ToString());
    }

    [Fact]
    public async Task CompleteUpload_WhenAttachmentExistsAndBelongsToUser_MarksUploaded()
    {
        var client = CreateAuthenticatedClient();
        var userId = client.GetUserId();

        var todo = new Todo("Task", "Desc", TodoPriority.High, userId);
        var attachment = Attachment.CreatePending(todo.Id, "test.pdf", "application/pdf");

        DbContext.Todos.Add(todo);
        DbContext.Attachments.Add(attachment);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var body = new
        {
            fileSize = 500L,
            isUploaded = true
        };

        var response = await client.PostAsJsonAsync(GetRoute(attachment.Id.Value), body, CurrentCancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var updated = await DbContext.Attachments
            .AsNoTracking()
            .SingleAsync(a => a.Id == attachment.Id, CurrentCancellationToken);

        Assert.Equal(AttachmentStatus.Uploaded, updated.Status);
        Assert.Equal(500L, updated.FileSize);
    }

    [Fact]
    public async Task CompleteUpload_WhenIsUploadedFalse_RemovesAttachment()
    {
        var client = CreateAuthenticatedClient();
        var userId = client.GetUserId();

        var todo = new Todo("Task", "Desc", TodoPriority.High, userId);
        var attachment = Attachment.CreatePending(todo.Id, "temp.txt", "text/plain");

        DbContext.Todos.Add(todo);
        DbContext.Attachments.Add(attachment);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var body = new
        {
            fileSize = 0L,
            isUploaded = false
        };

        var response = await client.PostAsJsonAsync(GetRoute(attachment.Id.Value), body, CurrentCancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var removed = await DbContext.Attachments
            .SingleOrDefaultAsync(a => a.Id == attachment.Id, CurrentCancellationToken);

        Assert.Null(removed);
    }

    [Fact]
    public async Task CompleteUpload_WhenAttachmentNotFound_ReturnsNotFound()
    {
        var client = CreateAuthenticatedClient();

        var body = new
        {
            fileSize = 100L,
            isUploaded = true
        };

        var response = await client.PostAsJsonAsync(GetRoute(Guid.NewGuid()), body, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problem = await response.ReadProblemAsync();
        Assert.Equal("Attachment.NotFound", problem.ErrorCode);
    }

    [Fact]
    public async Task CompleteUpload_WhenAttachmentBelongsToDifferentUser_ReturnsNotFound()
    {
        var client = CreateAuthenticatedClient();

        var todo = new Todo("Foreign", "XXX", TodoPriority.Low, "auth0|other");
        var attachment = Attachment.CreatePending(todo.Id, "private.docx", "application/vnd.ms-word");

        DbContext.Todos.Add(todo);
        DbContext.Attachments.Add(attachment);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var body = new { fileSize = 123L, isUploaded = true };
        var response = await client.PostAsJsonAsync(GetRoute(attachment.Id.Value), body, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problem = await response.ReadProblemAsync();
        Assert.Equal("Attachment.NotFound", problem.ErrorCode);
    }

    [Theory]
    [MemberData(nameof(InvalidFileSizes))]
    public async Task CompleteUpload_WhenInvalidFileSize_ReturnsBadRequest(
        long size)
    {
        var client = CreateAuthenticatedClient();
        var userId = client.GetUserId();

        var todo = new Todo("Valid Todo", "Desc", TodoPriority.Medium, userId);
        var attachment = Attachment.CreatePending(todo.Id, "file.txt", "text/plain");

        DbContext.Todos.Add(todo);
        DbContext.Attachments.Add(attachment);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var body = new
        {
            fileSize = size,
            isUploaded = true
        };

        var response = await client.PostAsJsonAsync(GetRoute(attachment.Id.Value), body, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problem = await response.ReadProblemAsync();
        Assert.Equal("Attachment.InvalidFileSize", problem.ErrorCode);
    }

    [Fact]
    public async Task CompleteUpload_WhenUserIsNotAuthenticated_ReturnsUnauthorized()
    {
        var client = CreateUnauthenticatedClient();

        var body = new { fileSize = 200L, isUploaded = true };

        var response = await client.PostAsJsonAsync(GetRoute(Guid.NewGuid()), body, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}