using Microsoft.AspNetCore.Http.HttpResults;
using Taskly.Shared.Pagination;
using Taskly.WebApi.Common.Shared.Pagination;
using Taskly.WebApi.Features.Tags.Endpoints;

namespace Taskly.WebApi.Features.Todos.Endpoints;

[Handler]
[MapGet(ApiRoutes.Todos.GetTodos)]
[Authorize(Policy = Security.Policies.User)]
public static partial class GetTodos
{
    internal static void CustomizeEndpoint(
        RouteHandlerBuilder endpoint)
    {
        endpoint.WithTags(nameof(Todo));
        endpoint.RequireRateLimiting(Security.RateLimiting.Global);
    }

    internal static Ok<PaginationResult<Response>> TransformResult(
        PaginationResult<Response> result) =>
        TypedResults.Ok(result);

    private static async ValueTask<PaginationResult<Response>> HandleAsync(
        PaginationQuery query,
        CurrentUserService currentUserService,
        PaginationService paginationService,
        CancellationToken ct)
    {
        var userId = currentUserService.GetUserId();

        return await paginationService.GetPaginationResultAsync(
            query,
            new GetTodosMapper(),
            q => q.Include(t => t.Tags)
                .Include(t => t.Attachments)
                .Where(t => t.UserId == userId),
            ct);
    }

    public sealed record Response(
        Guid Id,
        string Title,
        string? Description,
        int Priority,
        bool IsCompleted,
        DateTimeOffset CreatedAt,
        List<GetTags.Response> Tags,
        List<GetTodoAttachments> Attachments);

    public sealed record GetTodoAttachments(
        Guid Id,
        string FileName,
        long Size,
        string ContentType
    );
}

public sealed class GetTodosMapper : IPaginationMapper<Todo, GetTodos.Response>
{
    public List<GetTodos.Response> Map(List<Todo> source)
    {
        return source.Select(todo => new GetTodos.Response(
            todo.Id.Value,
            todo.Title,
            todo.Description,
            (int)todo.Priority,
            todo.IsCompleted,
            todo.CreatedAt,
            todo.Tags.Select(t => new GetTags.Response(t.Id.Value, t.Name)).ToList(),
            todo.Attachments.Select(attachment => new GetTodos.GetTodoAttachments(
                attachment.Id.Value,
                attachment.FileName,
                attachment.FileSize,
                attachment.ContentType
            )).ToList()
        )).ToList();
    }
}