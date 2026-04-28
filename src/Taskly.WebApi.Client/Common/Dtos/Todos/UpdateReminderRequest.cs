namespace Taskly.WebApi.Client.Common.Dtos.Todos;

public sealed record UpdateReminderRequest
{
    public required DateTime Deadline { get; init; }
    public required int ReminderOffsetInMinutes { get; init; }
}