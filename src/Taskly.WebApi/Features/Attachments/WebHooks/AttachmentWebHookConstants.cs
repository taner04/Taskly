namespace Taskly.WebApi.Features.Attachments.WebHooks;

internal static class AttachmentWebHookConstants
{
    internal const string RequestHeader = "X-WEBHOOK-SECRET-X";

    internal static class Event
    {
        internal const string UploadCompleted = "Attachment.Upload.Completed";
        internal const string UploadFailed = "Attachment.Upload.Failed";
    }
}