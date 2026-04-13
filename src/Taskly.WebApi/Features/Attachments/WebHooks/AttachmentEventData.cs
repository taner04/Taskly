namespace Taskly.WebApi.Features.Attachments.WebHooks;

public abstract record AttachmentEventData
{
    internal sealed record Completed(long FileSize) : AttachmentEventData;

    internal sealed record Failed : AttachmentEventData;
}