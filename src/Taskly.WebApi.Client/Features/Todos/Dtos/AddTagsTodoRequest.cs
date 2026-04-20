namespace Taskly.WebApi.Client.Features.Todos.Dtos;

public sealed record AddTagsTodoRequest
{
    public required List<Guid> TagIds { get; init; }
}