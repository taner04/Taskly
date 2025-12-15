using System.Net;
using Api.Features.Attachments.Endpoints;
using Api.Features.Attachments.Models;
using Api.Features.Todos.Model;
using FluentAssertions;

namespace IntegrationTests.Tests.Attachments;

public sealed class DownloadTests(TestingFixture fixture) : TestingBase(fixture)
{
    private static Todo CreateTodo(
        UserId userId)
    {
        return Todo.Create(
            "Test Todo",
            "Test Description",
            TodoPriority.Medium,
            userId
        );
    }

    private static Attachment CreateUploadedAttachment(
        Todo todo)
    {
        var attachment = Attachment.CreatePending(
            todo.Id,
            "testfile.txt",
            "text/plain"
        );

        attachment.MarkUploaded(1014);

        return attachment;
    }

    [Fact]
    public async Task DownloadAttachment_Should_Return401_When_Unauthenticated()
    {
        // Arrange
        var unauth = CreateUnauthenticatedClient();

        var randomId = AttachmentId.From(Guid.NewGuid());

        // Act
        var response = await unauth.DownloadAttachmentAsync(
            randomId,
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DownloadAttachment_Should_Return404_When_NotFound()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();

        var randomId = AttachmentId.From(Guid.NewGuid());

        // Act
        var response = await client.DownloadAttachmentAsync(
            randomId,
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        await response.ContainsErrorCode("Attachment.NotFound", CurrentCancellationToken);
    }

    [Fact]
    public async Task DownloadAttachment_Should_ReturnSasUrl_And_FileInfo()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();

        var userId = CurrentUserId;
        var todo = CreateTodo(userId);
        var attachment = CreateUploadedAttachment(todo);

        await using var dbContext = GetDbContext();
        dbContext.Add(todo);
        dbContext.Add(attachment);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        // Act
        var response = await client.DownloadAttachmentAsync(
            attachment.Id,
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.MapTo<Download.Response>(CurrentCancellationToken);

        result.Should().NotBeNull();
        result.DownloadUrl.Should().NotBeNullOrWhiteSpace();
        IsValidUrl(result.DownloadUrl).Should().BeTrue();
        result.FileName.Should().Be(attachment.FileName);
        result.ContentType.Should().Be(attachment.ContentType);
    }

    private static bool IsValidUrl(
        string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
               && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}