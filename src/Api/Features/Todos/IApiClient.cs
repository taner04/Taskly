// ReSharper disable CheckNamespace

using Api.Features.Todos.Endpoints;
using Refit;

namespace Api;

public partial interface IApiClient
{
    [Post(Routes.Todos.AddAttachment)]
    Task<AddAttachment.Dto> AddAttachmentAsync(
        TodoId todoId,
        [Body] AddAttachment.Command.CommandBody body,
        CancellationToken cancellationToken = default);

    [Post(Routes.Todos.AddTags)]
    Task AddTagsToTodoAsync(
        TodoId todoId,
        [Body] AddTags.Command.CommandBody body,
        CancellationToken cancellationToken = default);

    [Post(Routes.Todos.Complete)]
    Task CompleteTodoAsync(
        TodoId todoId,
        [Body] CompleteTodo.Command.CommandBody body,
        CancellationToken cancellationToken = default);

    [Post(Routes.Todos.Create)]
    Task<CreateTodo.Dto> CreateTodoAsync(
        [Body] CreateTodo.Command command,
        CancellationToken cancellationToken = default);

    [Get(Routes.Todos.GetTodos)]
    Task<List<GetTodos.TodoDto>> GetTodosAsync(CancellationToken cancellationToken = default);

    [Delete(Routes.Todos.RemoveAttachment)]
    Task RemoveAttachmentFromTodoAsync(
        TodoId todoId,
        AttachmentId attachmentId,
        CancellationToken cancellationToken = default);

    [Delete(Routes.Todos.RemoveTag)]
    Task RemoveTagFromTodoAsync(
        TodoId todoId,
        TagId tagId,
        CancellationToken cancellationToken = default);

    [Delete(Routes.Todos.Remove)]
    Task DeleteTodoAsync(
        TodoId todoId,
        CancellationToken cancellationToken = default);

    [Put(Routes.Todos.Update)]
    Task UpdateTodoAsync(
        TodoId todoId,
        [Body] UpdateTodo.Command.CommandBody body,
        CancellationToken cancellationToken = default);
}