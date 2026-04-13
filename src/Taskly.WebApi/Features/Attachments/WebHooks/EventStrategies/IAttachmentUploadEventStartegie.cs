namespace Taskly.WebApi.Features.Attachments.WebHooks.EventStrategies;

public interface IAttachmentUploadEventStartegie<in TEventData> : IAttachmentUploadEventStartegie
    where TEventData : AttachmentEventData
{
    Task HandleEventAsync(Attachment attachment, TEventData eventData, CancellationToken cancellationToken);
}

public interface IAttachmentUploadEventStartegie
{
    string EventType { get; }
}