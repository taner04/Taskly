namespace Taskly.WebApi.Client.Features.Todos.Dtos;

public sealed record CompleteTodoRequest
{
    public required bool Completed { get; init; }
}