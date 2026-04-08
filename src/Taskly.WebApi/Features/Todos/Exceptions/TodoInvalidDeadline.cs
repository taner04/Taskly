namespace Taskly.WebApi.Features.Todos.Exceptions;

internal sealed class TodoInvalidDeadline(string dateTime) :
    ModelBaseException(
        "Invalid Todo Deadline",
        $"The provided todo deadline '{dateTime}' is not in a valid datetime format.",
        "Todo.InvalidDeadline",
        HttpStatusCode.BadRequest);
