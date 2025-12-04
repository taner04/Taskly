using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Api.Features.Attachments.Endpoints;
using Api.Features.Attachments.Models;
using Api.Features.Todos.Model;
using Api.Shared.Api;
using IntegrationTests.Extensions;

namespace IntegrationTests.Tests.Attachments;

public class AddAttachmentTests(TestingFixture fixture) : TestingBase(fixture)
{
    public static TheoryData<string> AllowedExtensions =>
    [
        "json", "txt", "pdf",
        "png", "jpg", "jpeg",
        "docx", "pptx", "xlsx"
    ];

    public static TheoryData<long> InvalidSizes =>
    [
        -1L,
        0L,
        Attachment.MaxFileSizeInBytes + 1
    ];

    private static string AddRoute(
        Guid todoId)
    {
        return Routes.Todos.AddAttachment.Replace("{todoId:guid}", todoId.ToString());
    }

    private static string CompleteRoute(
        Guid attachmentId)
    {
        return Routes.Attachments.CompleteUpload.Replace("{attachmentId:guid}", attachmentId.ToString());
    }

    [Theory]
    [MemberData(nameof(AllowedExtensions))]
    public async Task AddAttachment_UploadToAzurite_ThenCompleteUpload(
        string extension)
    {
        var client = CreateAuthenticatedClient();
        var userId = client.GetUserId();

        var todo = new Todo("Task", "Desc", TodoPriority.Medium, userId);
        DbContext.Todos.Add(todo);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var addBody = new AddAttachment.Command.CommandBody
        {
            FileName = $"test.{extension}",
            ContentType = "application/octet-stream"
        };

        var addResponse = await client.PostAsJsonAsync(AddRoute(todo.Id.Value), addBody, CurrentCancellationToken);
        Assert.Equal(HttpStatusCode.OK, addResponse.StatusCode);

        var dto = await addResponse.Content.ReadFromJsonAsync<AddAttachment.Dto>(CurrentCancellationToken);
        Assert.NotNull(dto);

        //---------------------------------------------------------
        // Upload file bytes via SAS URL (real Azurite upload)
        //---------------------------------------------------------

        byte[] fileBytes = [1, 2, 3, 4];

        var uploadReq = new HttpRequestMessage(HttpMethod.Put, dto.UploadUrl)
        {
            Content = new ByteArrayContent(fileBytes)
        };

        uploadReq.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

        uploadReq.Headers.Add("x-ms-blob-type", "BlockBlob");

        var rawClient = new HttpClient();
        var uploadResponse = await rawClient.SendAsync(uploadReq, CurrentCancellationToken);

        Assert.True(uploadResponse.IsSuccessStatusCode);

        //---------------------------------------------------------
        // Complete Upload
        //---------------------------------------------------------

        var completeBody = new CompleteAttachmentUpload.Command.CommandBody
        {
            FileSize = fileBytes.Length,
            IsUploaded = true
        };

        var completeResponse = await client.PostAsJsonAsync(
            CompleteRoute(dto.AttachmentId),
            completeBody,
            CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.OK, completeResponse.StatusCode);

        //---------------------------------------------------------
        // Validate DB
        //---------------------------------------------------------

        var saved = await DbContext.Attachments
            .SingleAsync(a => a.Id == AttachmentId.From(dto.AttachmentId), CurrentCancellationToken);

        Assert.Equal(AttachmentStatus.Uploaded, saved.Status);
        Assert.Equal(fileBytes.Length, saved.FileSize);
    }

    // ------------------------------------------------------------
    // Negative Tests
    // ------------------------------------------------------------

    [Fact]
    public async Task CompleteUpload_WhenIsUploadedFalse_RemovesAttachment()
    {
        var client = CreateAuthenticatedClient();
        var userId = client.GetUserId();

        var todo = new Todo("TodoX", "Desc", TodoPriority.Medium, userId);
        DbContext.Todos.Add(todo);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var addBody = new AddAttachment.Command.CommandBody
        {
            FileName = "test.txt",
            ContentType = "text/plain"
        };

        var addResponse = await client.PostAsJsonAsync(AddRoute(todo.Id.Value), addBody, CurrentCancellationToken);

        var dto = await addResponse.Content.ReadFromJsonAsync<AddAttachment.Dto>(CurrentCancellationToken);

        var completeBody = new CompleteAttachmentUpload.Command.CommandBody
        {
            FileSize = new byte[1, 2, 3, 4].Length,
            IsUploaded = false
        };

        var completeResponse = await client.PostAsJsonAsync(
            CompleteRoute(dto!.AttachmentId),
            completeBody,
            CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.OK, completeResponse.StatusCode);

        var deleted = await DbContext.Attachments
            .SingleOrDefaultAsync(a => a.Id == AttachmentId.From(dto.AttachmentId), CurrentCancellationToken);

        Assert.Null(deleted);
    }

    [Fact]
    public async Task CompleteUpload_WhenAttachmentNotFound_ReturnsNotFound()
    {
        var client = CreateAuthenticatedClient();

        var body = new CompleteAttachmentUpload.Command.CommandBody
        {
            FileSize = 50L,
            IsUploaded = true
        };

        var response = await client.PostAsJsonAsync(
            CompleteRoute(Guid.NewGuid()),
            body,
            CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problem = await response.ReadProblemAsync();
        Assert.Equal("Attachment.NotFound", problem.ErrorCode);
    }

    [Fact]
    public async Task CompleteUpload_WhenAttachmentBelongsToAnotherUser_ReturnsNotFound()
    {
        var client = CreateAuthenticatedClient();

        var todo = new Todo("Foreign", "XXX", TodoPriority.Medium, "auth0|other");
        var attachment = Attachment.CreatePending(todo.Id, "file.txt", "text/plain");

        DbContext.Todos.Add(todo);
        DbContext.Attachments.Add(attachment);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var body = new CompleteAttachmentUpload.Command.CommandBody
        {
            FileSize = 10L,
            IsUploaded = true
        };

        var response = await client.PostAsJsonAsync(
            CompleteRoute(attachment.Id.Value),
            body,
            CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problem = await response.ReadProblemAsync();
        Assert.Equal("Attachment.NotFound", problem.ErrorCode);
    }

    [Theory]
    [MemberData(nameof(InvalidSizes))]
    public async Task CompleteUpload_WhenFileSizeInvalid_ReturnsBadRequest(
        long size)
    {
        var client = CreateAuthenticatedClient();
        var userId = client.GetUserId();

        var todo = new Todo("Todo", "Desc", TodoPriority.Medium, userId);
        var attachment = Attachment.CreatePending(todo.Id, "file.txt", "text/plain");

        DbContext.Todos.Add(todo);
        DbContext.Attachments.Add(attachment);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var body = new CompleteAttachmentUpload.Command.CommandBody
        {
            FileSize = size,
            IsUploaded = true
        };

        var response = await client.PostAsJsonAsync(
            CompleteRoute(attachment.Id.Value),
            body,
            CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problem = await response.ReadProblemAsync();
        Assert.Equal(HttpStatusCode.BadRequest, (HttpStatusCode)problem.Status!);
    }

    [Fact]
    public async Task CompleteUpload_WhenUserNotAuthenticated_ReturnsUnauthorized()
    {
        var client = CreateUnauthenticatedClient();

        var body = new CompleteAttachmentUpload.Command.CommandBody
        {
            FileSize = 10L,
            IsUploaded = true
        };

        var response = await client.PostAsJsonAsync(
            CompleteRoute(Guid.NewGuid()),
            body,
            CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}