namespace Taskly.Shared.WebApi.Responses.Attachments;

public sealed record DownloadAttachmentResponse(
    string DownloadUrl,
    string FileName);