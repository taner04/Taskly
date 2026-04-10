namespace Taskly.WebApi.Features.Todos.Exceptions;

internal sealed class InvalidTodoDescriptionException : TasklyException
{
    private InvalidTodoDescriptionException(
        string title,
        string message,
        string errorCode,
        HttpStatusCode statusCode) :
        base(title, message, errorCode, statusCode)
    {
    }

    public static void ThrowIfInvalid(string? title)
    {
        if (title?.Length is > Todo.MaxDescriptionLength or < Todo.MinDescriptionLength)
        {
            throw new InvalidTodoDescriptionException(
                title,
                $"The title of a todo must be between {Todo.MinTitleLength} and {Todo.MaxTitleLength} characters long.",
                "Todo.InvalidDescription",
                HttpStatusCode.BadRequest);
        }
    }
}