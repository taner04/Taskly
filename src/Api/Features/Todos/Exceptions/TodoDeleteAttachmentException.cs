namespace Api.Features.Todos.Exceptions;

public sealed class TodoDeleteAttachmentException(AttachmentId attachmentId) :
    ApiException(
        "Failed to delete attachment from todo.",
        $"An error occurred while deleting attachment with ID '{attachmentId}' from the todo.",
        "Todo.DeleteAttachmentError");