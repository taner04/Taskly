namespace Taskly.WebApi.Features.Attachments.WebHooks.EventStrategies;

public sealed class AttachmentEventDispatcher(
    ILogger<AttachmentEventDispatcher> logger,
    IServiceProvider serviceProvider)
{
    internal async Task HandleAsync(Attachment attachment, AttachmentEventData eventData, CancellationToken ct)
    {
        switch (eventData)
        {
            case AttachmentEventData.Completed completed:
                await serviceProvider
                    .GetRequiredService<IAttachmentUploadEventStartegie<AttachmentEventData.Completed>>()
                    .HandleEventAsync(attachment, completed, ct);
                break;
            case AttachmentEventData.Failed failed:
                await serviceProvider.GetRequiredService<IAttachmentUploadEventStartegie<AttachmentEventData.Failed>>()
                    .HandleEventAsync(attachment, failed, ct);
                break;
            default:
                logger.LogWarning("Received an unsupported attachment event data type: {EventDataType}",
                    eventData.GetType().FullName);
                break;
        }
    }
}