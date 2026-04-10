namespace Taskly.WebApi.Features.Todos.Exceptions;

internal sealed class InvalidTodoTitleException : TasklyException
{
    private InvalidTodoTitleException(
        string title,
        string message,
        string errorCode,
        HttpStatusCode statusCode) :
        base(title, message, errorCode, statusCode)
    {
    }

    public static void ThrowIfInvalid(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new InvalidTodoTitleException(
                title,
                "The title of a todo cannot be empty or consist only of whitespace.",
                "Todo.InvalidTitle",
                HttpStatusCode.BadRequest);
        }

        if (title.Length is > Todo.MaxTitleLength or < Todo.MinTitleLength)
        {
            throw new InvalidTodoTitleException(
                title,
                $"The title of a todo must be between {Todo.MinTitleLength} and {Todo.MaxTitleLength} characters long.",
                "Todo.InvalidTitle",
                HttpStatusCode.BadRequest);
        }
    }
}