using System.Net;
using Api.Features.Attachments.Endpoints;
using Api.Features.Attachments.Models;
using Api.Features.Todos.Model;
using FluentAssertions;

namespace IntegrationTests.Tests.Attachments;

public sealed class CompleteUploadTests(TestingFixture fixture) : TestingBase(fixture)
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

    private static Attachment CreatePendingAttachment(
        Todo todo)
    {
        return Attachment.CreatePending(
            todo.Id,
            "testfile.txt",
            "text/plain"
        );
    }

    [Fact]
    public async Task CompleteUpload_Should_Return401_When_Unauthenticated()
    {
        // Arrange
        var unauthenticatedClient = CreateUnauthenticatedClient();

        var randomId = AttachmentId.From(Guid.NewGuid());

        // Act
        var response = await unauthenticatedClient.CompleteAttachmentUploadAsync(
            randomId,
            new CompleteUpload.Command.CommandBody
            {
                FileSize = 500,
                IsUploaded = true
            },
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CompleteUpload_Should_Return404_When_AttachmentDoesNotExist()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();

        var randomId = AttachmentId.From(Guid.NewGuid());

        // Act
        var response = await client.CompleteAttachmentUploadAsync(
            randomId,
            new CompleteUpload.Command.CommandBody
            {
                FileSize = 500,
                IsUploaded = true
            },
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        await response.ContainsErrorCode("Attachment.NotFound", CurrentCancellationToken);
    }

    [Fact]
    public async Task CompleteUpload_Should_DeleteAttachment_When_IsUploadedIsFalse()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();

        var userId = CurrentUserId;
        var todo = CreateTodo(userId);
        var attachment = CreatePendingAttachment(todo);

        await using var dbContext = GetDbContext();
        dbContext.Add(todo);
        dbContext.Add(attachment);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        // Act
        var response = await client.CompleteAttachmentUploadAsync(
            attachment.Id,
            new CompleteUpload.Command.CommandBody
            {
                FileSize = 1234,
                IsUploaded = false
            },
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        (await GetDbContext().Attachments
                .AnyAsync(a => a.Id == attachment.Id, CurrentCancellationToken))
            .Should().BeFalse();
    }

    [Fact]
    public async Task CompleteUpload_Should_SetStatusUploaded_When_IsUploadedIsTrue()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();

        var userId = CurrentUserId;
        var todo = CreateTodo(userId);
        var attachment = CreatePendingAttachment(todo);

        await using var dbContext = GetDbContext();
        dbContext.Add(todo);
        dbContext.Add(attachment);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        // Act
        var response = await client.CompleteAttachmentUploadAsync(
            attachment.Id,
            new CompleteUpload.Command.CommandBody
            {
                FileSize = 2048,
                IsUploaded = true
            },
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updated = await GetDbContext().Attachments
            .AsNoTracking()
            .FirstAsync(a => a.Id == attachment.Id, CurrentCancellationToken);

        updated.Status.Should().Be(AttachmentStatus.Uploaded);
        updated.FileSize.Should().Be(2048);
    }

    [Fact]
    public async Task CompleteUpload_Should_Return400_When_FileSizeExceedsLimit()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();

        var userId = CurrentUserId;
        var todo = CreateTodo(userId);
        var attachment = CreatePendingAttachment(todo);

        await using var dbContext = GetDbContext();
        dbContext.Add(todo);
        dbContext.Add(attachment);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        // Act
        var response = await client.CompleteAttachmentUploadAsync(
            attachment.Id,
            new CompleteUpload.Command.CommandBody
            {
                FileSize = Attachment.MaxFileSizeInBytes + 1,
                IsUploaded = true
            },
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        await response.ContainsErrorCode("Attachment.InvalidFileSize", CurrentCancellationToken);
    }
}