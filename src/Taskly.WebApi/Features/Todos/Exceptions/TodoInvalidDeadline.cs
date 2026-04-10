using Taskly.WebApi.Common.Shared.Exceptions;

namespace Taskly.WebApi.Features.Todos.Exceptions;

internal sealed class TodoInvalidDeadline(string dateTime) :
    TasklyException(
        "Invalid Todo Deadline",
        $"The provided todo deadline '{dateTime}' is not in a valid datetime format.",
        "Todo.InvalidDeadline",
        HttpStatusCode.BadRequest);