using System.Net;
using Api.Features.Attachments.Models;
using Api.Features.Todos.Endpoints;
using Api.Features.Todos.Model;
using FluentAssertions;
using IntegrationTests.Extensions;

namespace IntegrationTests.Tests.Todos;

public sealed class AddAttachmentTests(TestingFixture fixture) : TestingBase(fixture)
{
    private static Todo CreateTodo(string userId)
    {
        return new Todo(
            "Test Todo",
            "Test Description",
            TodoPriority.Medium,
            userId
        );
    }

    [Fact]
    public async Task AddAttachment_Should_Return401_When_Unauthenticated()
    {
        // Arrange
        var unauthenticated = CreateUnauthenticatedClient();
        var randomTodoId = TodoId.From(Guid.NewGuid());

        // Act
        var response = await unauthenticated.AddAttachmentAsync(
            randomTodoId,
            new AddAttachment.Command.CommandBody
            {
                FileName = "test.txt",
                ContentType = "text/plain"
            },
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AddAttachment_Should_Return404_When_TodoDoesNotExist()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var randomTodoId = TodoId.From(Guid.NewGuid());

        // Act
        var response = await client.AddAttachmentAsync(
            randomTodoId,
            new AddAttachment.Command.CommandBody
            {
                FileName = "photo.png",
                ContentType = "image/png"
            },
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        await response.ContainsErrorCode("Todo.NotFound", CurrentCancellationToken);
    }

    [Fact]
    public async Task AddAttachment_Should_Return200_And_CreatePendingAttachment()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var userId = GetCurrentUserId();

        var todo = CreateTodo(userId);

        DbContext.Todos.Add(todo);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var fileName = "testfile.txt";
        var contentType = "text/plain";

        // Act
        var response = await client.AddAttachmentAsync(
            todo.Id,
            new AddAttachment.Command.CommandBody
            {
                FileName = fileName,
                ContentType = contentType
            },
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.MapTo<AddAttachment.Response>(CurrentCancellationToken);

        result.Should().NotBeNull();
        result.UploadUrl.Should().NotBeNullOrWhiteSpace();
        result.BlobPath.Should().NotBeNullOrWhiteSpace();

        // Validate database state
        var createdAttachment = await DbContext.Attachments
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == AttachmentId.From(result.AttachmentId), CurrentCancellationToken);

        createdAttachment.Should().NotBeNull();
        createdAttachment!.FileName.Should().Be(fileName);
        createdAttachment.ContentType.Should().Be(contentType);
        createdAttachment.Status.Should().Be(AttachmentStatus.Pending);
        createdAttachment.TodoId.Should().Be(todo.Id);
    }
}