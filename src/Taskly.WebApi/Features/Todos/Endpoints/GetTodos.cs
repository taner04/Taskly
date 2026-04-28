using Taskly.Shared.Pagination;
using Taskly.Shared.WebApi.Responses.Tags;
using Taskly.WebApi.Common.Shared.Pagination;

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

    internal static Ok<PaginationResult<GetTodoResponse>> TransformResult(
        PaginationResult<GetTodoResponse> result) =>
        TypedResults.Ok(result);

    private static async ValueTask<PaginationResult<GetTodoResponse>> HandleAsync(
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
}

public sealed class GetTodosMapper : IPaginationMapper<Todo, GetTodoResponse>
{
    public IEnumerable<GetTodoResponse> Map(IEnumerable<Todo> source)
    {
        return source.Select(todo => new GetTodoResponse(
            todo.Id.Value,
            todo.Title,
            todo.Description,
            (int)todo.Priority,
            todo.IsCompleted,
            todo.CreatedAt,
            todo.Tags.Select(t => new GetTagResponse(t.Id.Value, t.Name)).ToList(),
            todo.Attachments.Select(attachment => new GetTodoAttachments(
                attachment.Id.Value,
                attachment.FileName,
                attachment.FileSize,
                attachment.ContentType
            )).ToList()
        )).ToList();
    }
}