namespace Api.Features.Todos.Exceptions;

public sealed class TodoInvalidDeadlineException(
    DateTime? deadline,
    int? reminder,
    string details = "The provided todo schedule is invalid.")
    : ModelBaseException(
        title: "Invalid Todo Schedule",
        message: $"{details} Deadline: {deadline?.ToString("O") ?? "none"}, " +
                 $"Reminder minutes: {reminder?.ToString() ?? "none"}.",
        errorCode: "Todo.InvalidDeadline",
        statusCode: HttpStatusCode.BadRequest);