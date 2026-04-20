namespace Taskly.WebApi.Features.Attachments.WebHooks.EventStrategies.Strategies;

internal sealed class AttachmentUploadFailedEventStrategy(
    TasklyDbContext context,
    ILogger<AttachmentUploadFailedEventStrategy> logger) : IAttachmentUploadEventStartegie<AttachmentEventData.Failed>
{
    public string EventType => AttachmentWebHookConstants.Event.UploadFailed;

    public async Task HandleEventAsync(
        Attachment attachment,
        AttachmentEventData.Failed eventData,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Attachment upload failed for attachment with id: {AttachmentId}. Removing the attachment from the database.",
            attachment.Id);

        context.Attachments.Remove(attachment);

        await context.SaveChangesAsync(cancellationToken);
    }
}