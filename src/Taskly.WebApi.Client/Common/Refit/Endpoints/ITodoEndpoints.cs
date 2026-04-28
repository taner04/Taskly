using Taskly.WebApi.Client.Common.Dtos.Todos;

namespace Taskly.WebApi.Client.Common.Refit.Endpoints;

public interface ITodoEndpoints
{
    [Post(ApiRoutes.Todos.AddAttachment)]
    Task<HttpResponseMessage> AddAttachmentAsync(
        Guid todoId,
        [Body] AddAttachmentTodoRequest body,
        CancellationToken cancellationToken = default);

    [Post(ApiRoutes.Todos.AddTags)]
    Task<HttpResponseMessage> AddTagsToTodoAsync(
        Guid todoId,
        [Body] AddTagsTodoRequest body,
        CancellationToken cancellationToken = default);

    [Post(ApiRoutes.Todos.Complete)]
    Task<HttpResponseMessage> CompleteTodoAsync(
        Guid todoId,
        [Body] CompleteTodoRequest body,
        CancellationToken cancellationToken = default);

    [Post(ApiRoutes.Todos.Create)]
    Task<HttpResponseMessage> CreateTodoAsync(
        [Body] CreateTodoRequest command,
        CancellationToken cancellationToken = default);

    [Get(ApiRoutes.Todos.GetTodos)]
    Task<HttpResponseMessage> GetTodosAsync(
        PaginationQuery query,
        CancellationToken cancellationToken = default);

    [Delete(ApiRoutes.Todos.RemoveAttachment)]
    Task<HttpResponseMessage> RemoveAttachmentAsync(
        Guid todoId,
        Guid attachmentId,
        CancellationToken cancellationToken = default);

    [Delete(ApiRoutes.Todos.RemoveTag)]
    Task<HttpResponseMessage> RemoveTagFromTodoAsync(
        Guid todoId,
        Guid tagId,
        CancellationToken cancellationToken = default);

    [Delete(ApiRoutes.Todos.Remove)]
    Task<HttpResponseMessage> DeleteTodoAsync(
        Guid todoId,
        CancellationToken cancellationToken = default);

    [Put(ApiRoutes.Todos.Update)]
    Task<HttpResponseMessage> UpdateTodoAsync(
        Guid todoId,
        [Body] UpdateTodoRequest body,
        CancellationToken cancellationToken = default);

    [Delete(ApiRoutes.Todos.RemoveReminder)]
    Task<HttpResponseMessage> RemoveReminderAsync(
        Guid todoId,
        CancellationToken cancellationToken = default);

    [Put(ApiRoutes.Todos.UpdateReminder)]
    Task<HttpResponseMessage> UpdateReminderAsync(
        Guid todoId,
        [Body] UpdateReminderRequest body,
        CancellationToken cancellationToken = default);
}