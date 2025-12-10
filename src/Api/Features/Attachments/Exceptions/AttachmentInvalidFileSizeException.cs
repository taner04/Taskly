namespace Api.Features.Attachments.Exceptions;

public sealed class AttachmentInvalidFileSizeException(long fileSize) :
    ModelBaseException(
        "Invalid attachment file size.",
        $"Attachment file size '{fileSize}' exceeds the maximum allowed size.",
        "Attachment.InvalidFileSize",
        HttpStatusCode.BadRequest);