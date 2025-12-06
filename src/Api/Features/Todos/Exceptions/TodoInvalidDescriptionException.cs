namespace Api.Features.Todos.Exceptions;

public sealed class TodoInvalidDescriptionException(int currentLength)
    : ModelBaseException(
        "The todo description is invalid.",
        $"The todo description length of {currentLength} exceeds the maximum allowed length between {Todo.MinDescriptionLength} and {Todo.MaxDescriptionLength} characters.",
        "Todo.InvalidDescription",
        HttpStatusCode.BadRequest);