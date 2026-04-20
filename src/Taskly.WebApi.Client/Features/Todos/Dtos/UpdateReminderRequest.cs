namespace Taskly.WebApi.Client.Features.Todos.Dtos;

public sealed record UpdateReminderRequest
{
    public required DateTime Deadline { get; init; }
    public required int ReminderOffsetInMinutes { get; init; }
}