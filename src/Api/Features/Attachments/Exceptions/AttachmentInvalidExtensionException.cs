using Api.Shared.Exceptions;

namespace Api.Features.Attachments.Exceptions;

public sealed class AttachmentInvalidExtensionException(string extension) :
    ApiException(
        "The used file extension is not allowed.",
        $"The file extension '{extension}' is not permitted for attachments.",
        "Attachment.InvalidExtension");