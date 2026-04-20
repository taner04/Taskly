namespace Taskly.WebApi.Client.Features.Todos.Dtos;

public sealed record AddAttachmentTodoRequest
{
    public required string FileName { get; init; }
    public required string ContentType { get; init; }
}