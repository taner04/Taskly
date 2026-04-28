namespace Taskly.WebApi.Client.Common.Dtos.Todos;

public sealed record AddTagsTodoRequest
{
    public required List<Guid> TagIds { get; init; }
}