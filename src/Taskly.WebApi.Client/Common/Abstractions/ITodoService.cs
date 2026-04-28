using Taskly.Shared.WebApi.Responses.Todos;
using Taskly.WebApi.Client.Common.Dtos.Todos;

namespace Taskly.WebApi.Client.Common.Abstractions;

public interface ITodoService
{
    Task<WebClientResult<GetTodoResponse>> AddAttachmentAsync(
        Guid todoId,
        AddAttachmentTodoRequest request,
        string filePath,
        CancellationToken cancellationToken);

    Task<WebClientResult<GetTodoResponse>> AddTagAsync(
        Guid todoId,
        AddTagsTodoRequest request,
        CancellationToken cancellationToken);

    Task<WebClientResult<GetTodoResponse>> CompleteTodoAsync(
        Guid todoId,
        CompleteTodoRequest request,
        CancellationToken cancellationToken);

    Task<WebClientResult<GetTodoResponse>> CreateTodoAsync(
        CreateTodoRequest request,
        CancellationToken cancellationToken);

    Task<WebClientResult<GetTodoResponse>> DeleteTodoAsync(Guid todoId, CancellationToken cancellationToken);

    Task<WebClientResult<PaginationResult<GetTodoResponse>>> GetTodosAsync(
        PaginationQuery query,
        CancellationToken cancellationToken = default);

    Task<WebClientResult<GetTodoResponse>> RemoveAttachmentAsync(
        Guid todoId,
        Guid attachmentId,
        CancellationToken cancellationToken);

    Task<WebClientResult<GetTodoResponse>> RemoveReminderAsync(Guid todoId, CancellationToken cancellationToken);

    Task<WebClientResult<GetTodoResponse>> RemoveTagAsync(Guid todoId, Guid tagId, CancellationToken cancellationToken);

    Task<WebClientResult<GetTodoResponse>> UpdateReminderAsync(
        Guid todoId,
        UpdateReminderRequest request,
        CancellationToken cancellationToken);

    Task<WebClientResult<GetTodoResponse>> UpdateTodoAsync(
        Guid todoId,
        UpdateTodoRequest request,
        CancellationToken cancellationToken);
}