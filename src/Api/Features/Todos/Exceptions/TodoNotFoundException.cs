using Api.Features.Shared.Exceptions;

namespace Api.Features.Todos.Exceptions;

public sealed class TodoNotFoundException(TodoId todoId) :
    ApiException(
        "Could not find todo.",
        $"Todo with ID '{todoId.Value}' was not found.",
        "Todo.NotFound",
        HttpStatusCode.NotFound);