namespace Taskly.WebApi.Features.Todos.Common.Exceptions;

internal sealed class InvalidTodoDeadlineException : TasklyException
{
    private InvalidTodoDeadlineException(
        DateTime? deadline,
        int? reminder,
        string details = "The provided todo schedule is invalid.") : base("Invalid Todo Schedule",
        $"{details} Deadline: {deadline?.ToString("O") ?? "none"}, " +
        $"Reminder minutes: {reminder?.ToString() ?? "none"}.",
        "Todo.InvalidDeadline",
        HttpStatusCode.BadRequest)
    {
    }

    public static void ThrowIfInvalid(DateTime deadline, int reminder)
    {
        var now = DateTime.UtcNow;

        if (deadline <= now)
        {
            throw new InvalidTodoDeadlineException(
                deadline,
                reminder,
                "Deadline must be set to a future date and time.");
        }

        if (reminder < 0)
        {
            throw new InvalidTodoDeadlineException(
                deadline,
                reminder,
                "Reminder minutes cannot be negative.");
        }

        var reminderAt = deadline.AddMinutes(-reminder);
        if (reminderAt > deadline)
        {
            throw new InvalidTodoDeadlineException(
                deadline,
                reminder,
                "Reminder cannot occur after the deadline.");
        }

        if (reminder > (deadline - now).TotalMinutes)
        {
            throw new InvalidTodoDeadlineException(
                deadline,
                reminder,
                "Reminder cannot be further in the past than the time until deadline.");
        }
    }
}