using Taskly.WebApi.Features.Todos.Endpoints;
using TodoId = Taskly.WebApi.Features.Todos.Models.TodoId;

namespace Taskly.WebApi.Client.Abstractions.Endpoints;

public interface ITodoEndpoints
{
    [Post(ApiRoutes.Todos.AddAttachment)]
    Task<HttpResponseMessage> AddAttachmentAsync(
        TodoId todoId,
        [Body] AddAttachment.Command.CommandBody body,
        CancellationToken cancellationToken = default);

    [Post(ApiRoutes.Todos.AddTags)]
    Task<HttpResponseMessage> AddTagsToTodoAsync(
        TodoId todoId,
        [Body] AddTags.Command.CommandBody body,
        CancellationToken cancellationToken = default);

    [Post(ApiRoutes.Todos.Complete)]
    Task<HttpResponseMessage> CompleteTodoAsync(
        TodoId todoId,
        [Body] CompleteTodo.Command.CommandBody body,
        CancellationToken cancellationToken = default);

    [Post(ApiRoutes.Todos.Create)]
    Task<HttpResponseMessage> CreateTodoAsync(
        [Body] CreateTodo.Command command,
        CancellationToken cancellationToken = default);

    [Get(ApiRoutes.Todos.GetTodos)]
    Task<HttpResponseMessage> GetTodosAsync(
        GetTodos.Query query,
        CancellationToken cancellationToken = default);

    [Delete(ApiRoutes.Todos.RemoveAttachment)]
    Task<HttpResponseMessage> RemoveAttachmentFromTodoAsync(
        TodoId todoId,
        AttachmentId attachmentId,
        CancellationToken cancellationToken = default);

    [Delete(ApiRoutes.Todos.RemoveTag)]
    Task<HttpResponseMessage> RemoveTagFromTodoAsync(
        TodoId todoId,
        TagId tagId,
        CancellationToken cancellationToken = default);

    [Delete(ApiRoutes.Todos.Remove)]
    Task<HttpResponseMessage> RemoveTodoAsync(
        TodoId todoId,
        CancellationToken cancellationToken = default);

    [Put(ApiRoutes.Todos.Update)]
    Task<HttpResponseMessage> UpdateTodoAsync(
        TodoId todoId,
        UpdateTodo.Command.CommandBody body,
        CancellationToken cancellationToken = default);

    [Delete(ApiRoutes.Todos.RemoveReminder)]
    Task<HttpResponseMessage> RemoveReminderAsync(
        TodoId todoId,
        CancellationToken cancellationToken = default);

    [Put(ApiRoutes.Todos.UpdateReminder)]
    Task<HttpResponseMessage> UpdateReminderAsync(
        TodoId todoId,
        UpdateReminder.Command.CommandBody body,
        CancellationToken cancellationToken = default);
}