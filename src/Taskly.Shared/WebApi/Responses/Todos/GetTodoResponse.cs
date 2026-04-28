namespace Taskly.Shared.WebApi.Responses.Todos;

public sealed record GetTodoResponse(
    Guid Id,
    string Title,
    string? Description,
    int Priority,
    bool IsCompleted,
    DateTimeOffset CreatedAt,
    List<GetTagResponse> Tags,
    List<GetTodoAttachments> Attachments);

public sealed record GetTodoAttachments(
    Guid Id,
    string FileName,
    long Size,
    string ContentType
);