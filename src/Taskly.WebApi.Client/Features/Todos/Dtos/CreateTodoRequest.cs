namespace Taskly.WebApi.Client.Features.Todos.Dtos;

public sealed record CreateTodoRequest
{
    public required string Title { get; init; }
    public string? Description { get; init; }
    public required int Priority { get; init; }
}