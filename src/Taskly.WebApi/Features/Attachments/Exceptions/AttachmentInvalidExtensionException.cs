using Taskly.WebApi.Common.Shared.Exceptions;

namespace Taskly.WebApi.Features.Attachments.Exceptions;

internal sealed class AttachmentInvalidExtensionException(string extension) :
    TasklyException(
        "The used file extension is not allowed.",
        $"The file extension '{extension}' is not permitted for attachments.",
        "Attachment.InvalidExtension",
        HttpStatusCode.BadRequest);