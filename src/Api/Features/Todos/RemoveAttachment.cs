using Api.Features.Attachments.Models;
using Api.Features.Attachments.Services;
using Api.Features.Todos.Model;
using Api.Features.Users;
using Api.Shared.Features.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.Todos;

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

    internal static Results<Ok, NotFound<Error>, StatusCodeHttpResult>
        TransformResult(
            ErrorOr<Success> result)
    {
        return result.Match<
            Results<Ok, NotFound<Error>, StatusCodeHttpResult>>
        (
            _ => TypedResults.Ok(),
            errors =>
            {
                var e = errors.First();

                return e.Type switch
                {
                    ErrorType.NotFound => TypedResults.NotFound(e),
                    _ => TypedResults.StatusCode(500)
                };
            }
        );
    }

    private static async ValueTask<ErrorOr<Success>> HandleAsync(
        Command command,
        ApplicationDbContext db,
        CurrentUserService current,
        AttachmentService attachments,
        CancellationToken ct)
    {
        var userId = current.GetCurrentUserId();

        var todo = await db.Todos
            .Include(t => t.Attachments)
            .SingleOrDefaultAsync(t => t.Id == command.TodoId && t.UserId == userId, ct);

        if (todo is null)
        {
            return Error.NotFound("Todo.NotFound", "Todo not found.");
        }

        var attachment = todo.Attachments.SingleOrDefault(a => a.Id == command.AttachmentId);

        if (attachment is null)
        {
            return Error.NotFound("Attachment.NotFound", "Attachment not found.");
        }
        
        todo.Attachments.Remove(attachment);
        
        var deleted = await attachments.DeleteAsync(attachment, ct);

        if (!deleted)
        {
            return Error.Failure("Attachment.DeleteFailed", "Blob delete failed.");
        }

        return Result.Success;
    }

    [Validate]
    public sealed partial record Command : ITransactionalRequest, IValidationTarget<Command>
    {
        [NotEmpty] [FromRoute] public required TodoId TodoId { get; init; }
        [NotEmpty] [FromRoute] public required AttachmentId AttachmentId { get; init; }
    }
}