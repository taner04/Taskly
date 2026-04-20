namespace Taskly.WebApi.Features.Attachments.WebHooks.EventStrategies.Strategies;

internal sealed class AttachmentUploadCompletedEventStrategy(
    TasklyDbContext context,
    ILogger<AttachmentUploadFailedEventStrategy> logger)
    : IAttachmentUploadEventStartegie<AttachmentEventData.Completed>
{
    public string EventType => AttachmentWebHookConstants.Event.UploadCompleted;

    public async Task HandleEventAsync(
        Attachment attachment,
        AttachmentEventData.Completed eventData,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling attachment upload completed event for attachment with id: {AttachmentId}",
            attachment.Id);

        attachment.FileSize = eventData.FileSize;
        attachment.Status = AttachmentStatus.Uploaded;

        await context.SaveChangesAsync(cancellationToken);
    }
}