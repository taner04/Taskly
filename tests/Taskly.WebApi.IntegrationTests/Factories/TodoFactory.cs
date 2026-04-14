namespace Taskly.WebApi.IntegrationTests.Factories;

internal static class TodoFactory
{
    internal static Todo Create(
        UserId userId,
        string title = "Test Todo",
        string? description = "Test Description",
        TodoPriority priority = TodoPriority.Medium,
        bool isCompleted = false)
    {
        var todo = Todo.Create(title, description, priority, userId);

        if (isCompleted)
        {
            todo.IsCompleted = true;
        }

        return todo;
    }

    private static Todo CreateWithReminder(
        UserId userId,
        DateTime deadline,
        int reminderOffsetInMinutes)
    {
        var todo = Create(userId);
        todo.Deadline = deadline;
        todo.ReminderOffsetInMinutes = reminderOffsetInMinutes;
        todo.HangfireJobId = "1";

        return todo;
    }

    internal static Todo CreateWithReminder(UserId userId) =>
        CreateWithReminder(userId, DateTime.UtcNow.AddHours(2), 30);
}