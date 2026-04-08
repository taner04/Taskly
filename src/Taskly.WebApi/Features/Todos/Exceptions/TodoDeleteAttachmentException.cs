namespace Taskly.WebApi.Features.Todos.Exceptions;

internal sealed class TodoDeleteAttachmentException(AttachmentId attachmentId) :
    ModelBaseException(
        "Failed to delete attachment from todo.",
        $"An error occurred while deleting attachment with ID '{attachmentId}' from the todo.",
        "Todo.DeleteAttachmentError",
        HttpStatusCode.BadRequest);
