namespace Taskly.WebApi.Client.Common.Dtos.Todos;

public sealed record AddAttachmentTodoRequest
{
    public required string FileName { get; init; }
    public required string ContentType { get; init; }
}