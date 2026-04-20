namespace Taskly.WebApi.Features.Attachments.WebHooks;

public abstract record AttachmentEventData
{
    public sealed record Completed(long FileSize) : AttachmentEventData;

    public sealed record Failed : AttachmentEventData;
}