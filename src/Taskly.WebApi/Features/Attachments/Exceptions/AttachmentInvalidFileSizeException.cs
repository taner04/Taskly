using Taskly.WebApi.Common.Shared.Exceptions;

namespace Taskly.WebApi.Features.Attachments.Exceptions;

internal sealed class AttachmentInvalidFileSizeException(long fileSize) :
    TasklyException(
        "Invalid attachment file size.",
        $"Attachment file size '{fileSize}' exceeds the maximum allowed size.",
        "Attachment.InvalidFileSize",
        HttpStatusCode.BadRequest);