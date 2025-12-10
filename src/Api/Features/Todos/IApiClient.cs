// ReSharper disable CheckNamespace

using Api.Features.Todos.Endpoints;
using Refit;

namespace Api;

public partial interface IApiClient
{
    [Post(Routes.Todos.AddAttachment)]
    Task<HttpResponseMessage> AddAttachmentAsync(
        TodoId todoId,
        [Body] AddAttachment.Command.CommandBody body,
        CancellationToken cancellationToken = default);

    [Post(Routes.Todos.AddTags)]
    Task<HttpResponseMessage> AddTagsToTodoAsync(
        TodoId todoId,
        [Body] AddTags.Command.CommandBody body,
        CancellationToken cancellationToken = default);

    [Post(Routes.Todos.Complete)]
    Task<HttpResponseMessage> CompleteTodoAsync(
        TodoId todoId,
        [Body] CompleteTodo.Command.CommandBody body,
        CancellationToken cancellationToken = default);

    [Post(Routes.Todos.Create)]
    Task<HttpResponseMessage> CreateTodoAsync(
        [Body] CreateTodo.Command command,
        CancellationToken cancellationToken = default);

    [Get(Routes.Todos.GetTodos)]
    Task<HttpResponseMessage> GetTodosAsync(CancellationToken cancellationToken = default);

    [Delete(Routes.Todos.RemoveAttachment)]
    Task<HttpResponseMessage> RemoveAttachmentFromTodoAsync(
        TodoId todoId,
        AttachmentId attachmentId,
        CancellationToken cancellationToken = default);

    [Delete(Routes.Todos.RemoveTag)]
    Task<HttpResponseMessage> RemoveTagFromTodoAsync(
        TodoId todoId,
        TagId tagId,
        CancellationToken cancellationToken = default);

    [Delete(Routes.Todos.Remove)]
    Task<HttpResponseMessage> RemoveTodoAsync(
        TodoId todoId,
        CancellationToken cancellationToken = default);

    [Put(Routes.Todos.Update)]
    Task<HttpResponseMessage> UpdateTodoAsync(
        TodoId todoId,
        [Body] UpdateTodo.Command.CommandBody body,
        CancellationToken cancellationToken = default);
    
    [Delete(Routes.Todos.RemoveReminder)]
    Task<HttpResponseMessage> RemoveReminderAsync(
        TodoId todoId,
        CancellationToken cancellationToken = default);
    
    [Put(Routes.Todos.UpdateReminder)]
    Task<HttpResponseMessage> UpdateReminderAsync(
        TodoId todoId,
        [Body] UpdateReminder.Command.CommandBody body,
        CancellationToken cancellationToken = default);
    
}