namespace Api.Features.Todos.Exceptions;

public sealed class TodoInvalidScheduleException(
    DateTime? deadline,
    int? reminder,
    string details = "The provided todo schedule is invalid.")
    : ModelBaseException(
        title: "Invalid Todo Schedule",
        message: $"{details} Deadline: {deadline?.ToString("O") ?? "none"}, " +
                 $"Reminder minutes: {reminder?.ToString() ?? "none"}.",
        errorCode: "Todo.InvalidSchedule",
        statusCode: HttpStatusCode.BadRequest);