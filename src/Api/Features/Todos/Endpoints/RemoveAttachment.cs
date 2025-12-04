using Api.Features.Attachments.Exceptions;
using Api.Features.Attachments.Models;
using Api.Features.Attachments.Services;
using Api.Features.Todos.Exceptions;
using Api.Features.Todos.Model;
using Api.Features.Users.Services;
using Api.Shared.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.Todos.Endpoints;

[Handler]
[MapDelete(Routes.Todos.RemoveAttachment)]
[Authorize]
public static partial class RemoveAttachment
{
    internal static void CustomizeEndpoint(
        IEndpointConventionBuilder endpoint)
    {
        endpoint.WithTags(nameof(Todo));
    }

    internal static NoContent TransformResult(
        ValueTask result)
    {
        return TypedResults.NoContent();
    }

    private static async ValueTask HandleAsync(
        Command command,
        ApplicationDbContext context,
        CurrentUserService current,
        AttachmentService attachments,
        CancellationToken ct)
    {
        var userId = current.GetCurrentUserId();

        var todo = await context.Todos
            .Include(t => t.Attachments)
            .SingleOrDefaultAsync(t => t.Id == command.TodoId && t.UserId == userId, ct);

        if (todo is null)
        {
            throw new TodoNotFoundException(command.TodoId);
        }

        var attachment = todo.Attachments.SingleOrDefault(a => a.Id == command.AttachmentId);

        if (attachment is null)
        {
            throw new AttachmentNotFoundException(command.AttachmentId);
        }

        todo.Attachments.Remove(attachment);

        if (!await attachments.DeleteAsync(attachment, ct))
        {
            throw new TodoDeleteAttachmentException(command.AttachmentId);
        }

        await context.SaveChangesAsync(ct);
    }

    [Validate]
    public sealed partial record Command : IValidationTarget<Command>
    {
        [NotEmpty] [FromRoute] public required TodoId TodoId { get; init; }
        [NotEmpty] [FromRoute] public required AttachmentId AttachmentId { get; init; }
    }
}