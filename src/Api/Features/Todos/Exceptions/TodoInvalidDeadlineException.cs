namespace Api.Features.Todos.Exceptions;

public sealed class TodoInvalidDeadlineException(
    DateTime? deadline,
    int? reminder,
    string details = "The provided todo schedule is invalid.")
    : ModelBaseException(
        "Invalid Todo Schedule",
        $"{details} Deadline: {deadline?.ToString("O") ?? "none"}, " +
        $"Reminder minutes: {reminder?.ToString() ?? "none"}.",
        "Todo.InvalidDeadline",
        HttpStatusCode.BadRequest);