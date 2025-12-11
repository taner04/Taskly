using Api.Features.Shared.Dtos.Tags;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Features.Todos.Endpoints;

[Handler]
[MapGet(Routes.Todos.GetTodos)]
[Authorize(Policy = Policies.User)]
public static partial class GetTodos
{
    internal static void CustomizeEndpoint(
        IEndpointConventionBuilder endpoint)
    {
        endpoint.WithTags(nameof(Todo));
    }

    internal static Ok<List<Response>> TransformResult(
        List<Response> result)
    {
        return TypedResults.Ok(result);
    }

    private static async ValueTask<List<Response>> HandleAsync(
        Query _,
        ApplicationDbContext context,
        CurrentUserService currentUserService,
        CancellationToken ct)
    {
        var userId = currentUserService.GetUserId();
        var todos = await context.Todos
            .Include(t => t.Tags)
            .Include(t => t.Attachments)
            .Where(t => t.UserId == userId).ToListAsync(ct);

        return todos.Select(Response.FromDomain).ToList();
    }

    public sealed record Query;

    public sealed record AttachmentDto(
        Guid Id,
        string FileName,
        long Size,
        string ContentType,
        string DownloadUrl
    )
    {
        public static AttachmentDto FromDomain(
            Attachment attachment)
        {
            return new AttachmentDto(
                attachment.Id.Value,
                attachment.FileName,
                attachment.FileSize,
                attachment.ContentType,
                attachment.GetDownloadUrl()
            );
        }
    }

    public sealed record Response(
        Guid Id,
        string Title,
        string? Description,
        TodoPriority Priority,
        bool IsCompleted,
        List<TagDto> Tags,
        List<AttachmentDto> Attachments,
        Guid UserId
    )
    {
        public static Response FromDomain(
            Todo todo)
        {
            return new Response(
                todo.Id.Value,
                todo.Title,
                todo.Description,
                todo.Priority,
                todo.IsCompleted,
                todo.Tags.Select(TagDto.FromDomain).ToList(),
                todo.Attachments.Select(AttachmentDto.FromDomain).ToList(),
                todo.UserId.Value
            );
        }
    }
}