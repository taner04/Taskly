namespace Taskly.WebApi.Features.Attachments.Exceptions;

internal sealed class AttachmentInvalidExtensionException(string extension) :
    ModelBaseException(
        "The used file extension is not allowed.",
        $"The file extension '{extension}' is not permitted for attachments.",
        "Attachment.InvalidExtension",
        HttpStatusCode.BadRequest);
