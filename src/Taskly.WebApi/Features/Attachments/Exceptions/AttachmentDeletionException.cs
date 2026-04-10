namespace Taskly.WebApi.Features.Attachments.Exceptions;

internal sealed class AttachmentDeletionException(AttachmentId attachmentId) :
    TasklyException(
        "Failed to delete attachment from todo.",
        $"An error occurred while deleting attachment with ID '{attachmentId}' from the todo.",
        "Todo.DeleteAttachmentError",
        HttpStatusCode.BadRequest);