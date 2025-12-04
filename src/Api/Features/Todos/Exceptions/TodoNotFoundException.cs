using Api.Features.Todos.Model;
using Api.Shared.Exceptions;

namespace Api.Features.Todos.Exceptions;

public sealed class TodoNotFoundException(TodoId todoId) :
    ApiException(
        "Could not find todo.",
        $"Todo with ID '{todoId.Value}' was not found.",
        "Todo.NotFound");