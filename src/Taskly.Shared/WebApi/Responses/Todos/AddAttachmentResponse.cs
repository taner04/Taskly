namespace Taskly.Shared.WebApi.Responses.Todos;

public sealed record AddAttachmentResponse(
    GetTodoResponse TodoResponse,
    string UploadUrl);