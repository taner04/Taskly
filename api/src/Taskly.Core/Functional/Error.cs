namespace Taskly.Core.Functional;

public readonly struct Error
{
    private Error(string title, string message, int code, Dictionary<string, List<string>>? validationErrors = null)
    {
        Title = title;
        Message = message;
        Code = code;
        ValidationErrors = validationErrors ?? [];
    }

    public string Title { get; init; }
    public string Message { get; init; }
    public int Code { get; init; }
    public Dictionary<string, List<string>> ValidationErrors { get; init; } 

    public static Error Validation(string title, string message, Dictionary<string, List<string>> validationErrors)
        => new (title, message, 400, validationErrors);

    public static Error NotFound(string title, string message)
        => new (title, message, 404);

    public static Error Unauthorized(string title, string message)
        => new (title, message, 401);

    public static Error Internal(string title, string message)
        => new (title, message, 500);

    public static Error Conflict(string title, string message)
        => new (title, message, 409);
}