namespace Taskly.WebApi.Features.Attachments.Exceptions;

internal sealed class AttachmentInvalidFileSizeException(long fileSize) :
    ModelBaseException(
        "Invalid attachment file size.",
        $"Attachment file size '{fileSize}' exceeds the maximum allowed size.",
        "Attachment.InvalidFileSize",
        HttpStatusCode.BadRequest);
