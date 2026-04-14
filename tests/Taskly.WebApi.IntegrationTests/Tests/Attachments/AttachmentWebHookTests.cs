using System.Text.Json;
using Taskly.WebApi.Features.Attachments.WebHooks;

namespace Taskly.WebApi.IntegrationTests.Tests.Attachments;

public sealed class AttachmentWebHookTests(TestingFixture fixture) : TestingBase(fixture)
{
    private static AttachmentEventWebHook.Command CreateUploadCompletedCommand() =>
        new()
        {
            Body = new AttachmentEventWebHook.Command.CommandBody
            {
                AttachmentId = Guid.NewGuid(),
                EventType = AttachmentWebHookConstants.Event.UploadCompleted,
                EventData = JsonSerializer.SerializeToElement(new { fileSize = 1024000L })
            }
        };

    [Fact]
    public async Task WebHook_Should_Return401_When_SecretKeyMissing()
    {
        // Arrange
        var client = GetUnauthenticatedClient();
        var command = CreateUploadCompletedCommand();

        // Act
        var response = await client.ReceiveAttachmentWebHookAsync(
            command,
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task WebHook_Should_Return401_When_SecretKeyInvalid()
    {
        // Arrange 
        var client = GetUnauthenticatedClient();

        var command = CreateUploadCompletedCommand();

        // Act
        var response = await client.ReceiveAttachmentWebHookAsync(
            command,
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task WebHook_Should_UpdateAttachmentStatus_When_UploadCompletedEventReceived()
    {
        // Arrange
        var userId = CurrentUserId;
        var todo = TodoFactory.Create(userId);
        var attachment = AttachmentFactory.CreatePending(todo);

        await using var dbContext = GetDbContext();
        dbContext.Add(todo);
        dbContext.Add(attachment);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        var command = new AttachmentEventWebHook.Command
        {
            Body = new AttachmentEventWebHook.Command.CommandBody
            {
                AttachmentId = attachment.Id.Value,
                EventType = AttachmentWebHookConstants.Event.UploadCompleted,
                EventData = JsonSerializer.SerializeToElement(new { fileSize = 1024000L })
            }
        };

        var client = GetWebHookClient();

        // Act
        var response = await client.ReceiveAttachmentWebHookAsync(
            command,
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedAttachment = await dbContext.Attachments
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == attachment.Id, CurrentCancellationToken);

        updatedAttachment.Should().NotBeNull();
        updatedAttachment!.Status.Should().Be(AttachmentStatus.Uploaded);
        updatedAttachment.FileSize.Should().Be(1024000L);
    }

    [Fact]
    public async Task WebHook_Should_DeleteAttachment_When_UploadFailedEventReceived()
    {
        // Arrange
        var userId = CurrentUserId;
        var todo = TodoFactory.Create(userId);
        var attachment = AttachmentFactory.CreatePending(todo);

        await using var dbContext = GetDbContext();
        dbContext.Add(todo);
        dbContext.Add(attachment);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        var command = new AttachmentEventWebHook.Command
        {
            Body = new AttachmentEventWebHook.Command.CommandBody
            {
                AttachmentId = attachment.Id.Value,
                EventType = AttachmentWebHookConstants.Event.UploadFailed,
                EventData = JsonSerializer.SerializeToElement(new { })
            }
        };

        var client = GetWebHookClient();

        // Act
        var response = await client.ReceiveAttachmentWebHookAsync(
            command,
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var deletedAttachment = await dbContext.Attachments
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == attachment.Id, CurrentCancellationToken);

        deletedAttachment.Should().BeNull();
    }

    [Fact]
    public async Task WebHook_Should_Return404_When_AttachmentNotFound()
    {
        // Arrange
        var randomAttachmentId = AttachmentId.From(Guid.NewGuid());

        var command = new AttachmentEventWebHook.Command
        {
            Body = new AttachmentEventWebHook.Command.CommandBody
            {
                AttachmentId = randomAttachmentId.Value,
                EventType = AttachmentWebHookConstants.Event.UploadCompleted,
                EventData = JsonSerializer.SerializeToElement(new { fileSize = 1024000L })
            }
        };

        var client = GetWebHookClient();

        // Act
        var response = await client.ReceiveAttachmentWebHookAsync(
            command,
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}