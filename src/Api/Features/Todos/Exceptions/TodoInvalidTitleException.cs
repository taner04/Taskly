namespace Api.Features.Todos.Exceptions;

public class TodoInvalidTitleException(int currentLength) :
    ApiException(
        "The todo description is invalid.",
        $"The todo description length of {currentLength} exceeds the maximum allowed length between {Todo.MinTitleLength} and {Todo.MaxTitleLength} characters.",
        "Todo.InvalidDescription");