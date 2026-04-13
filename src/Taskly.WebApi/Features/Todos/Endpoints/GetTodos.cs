using Microsoft.AspNetCore.Http.HttpResults;
using Taskly.WebApi.Common.Shared.Pagination;

namespace Taskly.WebApi.Features.Todos.Endpoints;

[Handler]
[MapGet(ApiRoutes.Todos.GetTodos)]
[Authorize(Policy = Policies.Roles.User)]
public static partial class GetTodos
{
    internal static void CustomizeEndpoint(
        RouteHandlerBuilder endpoint)
    {
        endpoint.WithTags(nameof(Todo));
        endpoint.RequireRateLimiting(Policies.RateLimiting.Global);
    }

    internal static Ok<PaginationResult<Response>> TransformResult(
        PaginationResult<Response> result) =>
        TypedResults.Ok(result);

    private static async ValueTask<PaginationResult<Response>> HandleAsync(
        Query query,
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

    public sealed record Query(
        int PageIndex,
        int
            PageSize) : PaginationQuery(PageIndex, PageSize);

    public sealed record AttachmentDto(
        Guid Id,
        string FileName,
        long Size,
        string ContentType,
        string DownloadUrl
    );

    public sealed record Response(
        Guid Id,
        string Title,
        string? Description,
        TodoPriority Priority,
        bool IsCompleted,
        List<TagDto> Tags,
        List<AttachmentDto> Attachments,
        Guid UserId);
}

public sealed class GetTodosMapper : IPaginationMapper<Todo, GetTodos.Response>
{
    public List<GetTodos.Response> Map(List<Todo> source)
    {
        return source.Select(todo => new GetTodos.Response(
            todo.Id.Value,
            todo.Title,
            todo.Description,
            todo.Priority,
            todo.IsCompleted,
            todo.Tags.Select(TagDto.FromDomain).ToList(),
            todo.Attachments.Select(attachment => new GetTodos.AttachmentDto(
                attachment.Id.Value,
                attachment.FileName,
                attachment.FileSize,
                attachment.ContentType,
                $"/attachments/{attachment.Id.Value}/download"
            )).ToList(),
            todo.UserId.Value
        )).ToList();
    }
}