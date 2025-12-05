using Api.Features.Shared.Exceptions;

namespace Api.Features.Attachments.Exceptions;

public sealed class AttachmentNotFoundException(AttachmentId attachmentId)
    : ApiException(
        "Could not find attachment.",
        $"Attachment with ID '{attachmentId}' was not found.",
        "Attachment.NotFound",
        HttpStatusCode.NotFound);