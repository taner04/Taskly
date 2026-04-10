using Taskly.WebApi.Common.Shared.Exceptions;

namespace Taskly.WebApi.Features.Todos.Exceptions;

internal sealed class AttachmentDeletionFailedException(AttachmentId attachmentId) :
    TasklyException(
        "Failed to delete attachment from todo.",
        $"An error occurred while deleting attachment with ID '{attachmentId}' from the todo.",
        "Todo.DeleteAttachmentError",
        HttpStatusCode.BadRequest);