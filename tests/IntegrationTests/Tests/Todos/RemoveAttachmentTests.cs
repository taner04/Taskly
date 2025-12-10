using System.Net;
using Api.Features.Attachments.Models;
using Api.Features.Todos.Model;
using FluentAssertions;
using IntegrationTests.Extensions;

namespace IntegrationTests.Tests.Todos;

public sealed class RemoveAttachmentTests(TestingFixture fixture) : TestingBase(fixture)
{
    private static Todo CreateTodo(UserId userId)
    {
        return new Todo(
            "Test Todo",
            "Test Description",
            TodoPriority.Medium,
            userId);
    }

    private static Attachment CreateAttachment(Todo todo, string fileName = "file.txt",
        string contentType = "text/plain")
    {
        return Attachment.CreatePending(
            todo.Id,
            fileName,
            contentType);
    }

    [Fact]
    public async Task RemoveAttachment_Should_Return401_When_Unauthenticated()
    {
        // Arrange
        var client = CreateUnauthenticatedClient();

        // Act
        var response = await client.RemoveAttachmentFromTodoAsync(
            TodoId.From(Guid.NewGuid()),
            AttachmentId.From(Guid.NewGuid()),
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RemoveAttachment_Should_Return404_When_TodoDoesNotExist()
    {
        // Arrange
        var client = CreateAuthenticatedClient();

        // Act
        var response = await client.RemoveAttachmentFromTodoAsync(
            TodoId.From(Guid.NewGuid()),
            AttachmentId.From(Guid.NewGuid()),
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        await response.ContainsErrorCode("Todo.NotFound", CurrentCancellationToken);
    }

    [Fact]
    public async Task RemoveAttachment_Should_Return404_When_AttachmentDoesNotExist()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var userId = GetCurrentUserId();
        var todo = CreateTodo(userId);

        DbContext.Add(todo);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        // Act
        var response = await client.RemoveAttachmentFromTodoAsync(
            todo.Id,
            AttachmentId.From(Guid.NewGuid()), // nonexistent
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        await response.ContainsErrorCode("Attachment.NotFound", CurrentCancellationToken);
    }

    [Fact]
    public async Task RemoveAttachment_Should_Return200_And_RemoveAttachment()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var userId = GetCurrentUserId();

        var todo = CreateTodo(userId);
        var attachment = CreateAttachment(todo);

        todo.Attachments.Add(attachment);

        DbContext.Add(todo);
        DbContext.Add(attachment);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var container = GetService<BlobServiceClient>().GetBlobContainerClient("attachments");

        await container.CreateIfNotExistsAsync(cancellationToken: CurrentCancellationToken);

        var blob = container.GetBlobClient(attachment.BlobName);
        await blob.UploadAsync(
            new BinaryData("fake file"),
            true,
            CurrentCancellationToken);

        // Act
        var response = await client.RemoveAttachmentFromTodoAsync(
            todo.Id,
            attachment.Id,
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var exists = await DbContext.Attachments
            .AnyAsync(a => a.Id == attachment.Id, CurrentCancellationToken);

        exists.Should().BeFalse();
    }
}