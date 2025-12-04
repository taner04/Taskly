using Api.Features.Shared.Exceptions;

namespace Api.Features.Attachments.Exceptions;

public sealed class AttachmentInvalidFileSizeException(long fileSize) :
    ApiException(
        "Invalid attachment file size.",
        $"Attachment file size '{fileSize}' exceeds the maximum allowed size.",
        "Attachment.InvalidFileSize");