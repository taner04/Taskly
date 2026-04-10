using Taskly.WebApi.Common.Shared.Exceptions;

namespace Taskly.WebApi.Features.Todos.Exceptions;

internal sealed class TodoInvalidDeadlineException(
    DateTime? deadline,
    int? reminder,
    string details = "The provided todo schedule is invalid.")
    : TasklyException(
        "Invalid Todo Schedule",
        $"{details} Deadline: {deadline?.ToString("O") ?? "none"}, " +
        $"Reminder minutes: {reminder?.ToString() ?? "none"}.",
        "Todo.InvalidDeadline",
        HttpStatusCode.BadRequest);