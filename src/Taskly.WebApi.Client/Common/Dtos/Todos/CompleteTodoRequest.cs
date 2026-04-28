namespace Taskly.WebApi.Client.Common.Dtos.Todos;

public sealed record CompleteTodoRequest
{
    public required bool Completed { get; init; }
}