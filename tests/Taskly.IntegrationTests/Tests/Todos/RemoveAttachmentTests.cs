using Taskly.ServiceDefaults;

namespace Taskly.IntegrationTests.Tests.Todos;

public sealed class RemoveAttachmentTests(TestingFixture fixture) : TestingBase(fixture)
{
    private static Todo CreateTodo(
        UserId userId) =>
        Todo.Create(
            "Test Todo",
            "Test Description",
            TodoPriority.Medium,
            userId);

    private static Attachment CreateAttachment(
        Todo todo,
        string fileName = "file.txt",
        string contentType = "text/plain") =>
        Attachment.Create(
            todo.Id,
            fileName,
            contentType);

    [Fact]
    public async Task RemoveAttachment_Should_Return401_When_Unauthenticated()
    {
        // Arrange
        var client = GetUnauthenticatedClient();

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
        var client = CreateAuthenticatedUserClient();

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
        var client = CreateAuthenticatedUserClient();
        var userId = CurrentUserId;
        var todo = CreateTodo(userId);

        await using var dbContext = GetDbContext();
        dbContext.Add(todo);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

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
        var client = CreateAuthenticatedUserClient();
        var userId = CurrentUserId;

        var todo = CreateTodo(userId);
        var attachment = CreateAttachment(todo);

        todo.Attachments.Add(attachment);

        await using var dbContext = GetDbContext();
        dbContext.Add(todo);
        dbContext.Add(attachment);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        var container = GetService<BlobServiceClient>().GetBlobContainerClient(AppHostConstants.AzureBlobContainerName);

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

        await using var verifyContext = GetDbContext();
        var exists = await verifyContext.Attachments
            .AnyAsync(a => a.Id == attachment.Id, CurrentCancellationToken);

        exists.Should().BeFalse();
    }
}