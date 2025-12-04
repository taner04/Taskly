using Api.Features.Attachments.Models;
using Api.Features.Todos.Model;
using Api.Features.Users.Services;
using Api.Shared.Api;
using Api.Shared.Dtos.Tags;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Features.Todos.Endpoints;

[Handler]
[MapGet(Routes.Todos.GetTodos)]
[Authorize]
public static partial class GetTodos
{
    internal static void CustomizeEndpoint(
        IEndpointConventionBuilder endpoint)
    {
        endpoint.WithTags(nameof(Todo));
    }

    internal static Ok<List<TodoDto>> TransformResult(
        List<TodoDto> result)
    {
        return TypedResults.Ok(result);
    }

    private static async ValueTask<List<TodoDto>> HandleAsync(
        Query _,
        ApplicationDbContext context,
        CurrentUserService currentUserService,
        CancellationToken ct)
    {
        var userId = currentUserService.GetCurrentUserId();
        var todos = await context.Todos.Where(t => t.UserId == userId).ToListAsync(ct);

        return todos.Select(TodoDto.FromDomain).ToList();
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

    public sealed record TodoDto(
        Guid Id,
        string Title,
        string? Description,
        TodoPriority Priority,
        bool IsCompleted,
        List<TagDto> Tags,
        List<AttachmentDto> Attachments,
        string UserId
    )
    {
        public static TodoDto FromDomain(
            Todo todo)
        {
            return new TodoDto(
                todo.Id.Value,
                todo.Title,
                todo.Description,
                todo.Priority,
                todo.IsCompleted,
                todo.Tags.Select(TagDto.FromDomain).ToList(),
                todo.Attachments.Select(AttachmentDto.FromDomain).ToList(),
                todo.UserId
            );
        }
    }
}