using Taskly.WebApi.Features.Attachments.Exceptions;
using Taskly.WebApi.Features.Attachments.Services;
using Taskly.WebApi.Features.Todos.Specifications;

namespace Taskly.WebApi.Features.Todos.Endpoints;

[Handler]
[MapDelete(ApiRoutes.Todos.RemoveAttachment)]
[Authorize(Policy = Security.Policies.User)]
public static partial class RemoveAttachment
{
    internal static void CustomizeEndpoint(
        RouteHandlerBuilder endpoint)
    {
        endpoint.WithTags(nameof(Todo));
        endpoint.RequireRateLimiting(Security.RateLimiting.Global);
    }

    private static async ValueTask HandleAsync(
        [AsParameters] Command command,
        TasklyDbContext context,
        CurrentUserService currentUserService,
        AttachmentBlobContainerService attachmentBlobContainerService,
        CancellationToken ct)
    {
        var userId = currentUserService.GetUserId();

        var spec = new TodoByUserIdSpecificationWithAttachmentsSpec(command.TodoId, userId);
        var todo = await context.Todos
            .WithSpecification(spec)
            .SingleOrDefaultAsync(ct) ?? throw new ModelNotFoundException<Todo>(command.TodoId.Value);

        var attachment = todo.Attachments.SingleOrDefault(a => a.Id == command.AttachmentId) ??
                         throw new ModelNotFoundException<Attachment>(command.AttachmentId.Value);

        await using var transaction = await context.Database.BeginTransactionAsync(ct);

        try
        {
            todo.Attachments.Remove(attachment);

            if (!await attachmentBlobContainerService.DeleteAsync(attachment, ct))
            {
                throw new AttachmentDeletionException(command.AttachmentId);
            }

            await context.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    [Validate]
    public sealed partial record Command : IValidationTarget<Command>
    {
        [NotEmpty] [FromRoute] public required TodoId TodoId { get; init; }
        [NotEmpty] [FromRoute] public required AttachmentId AttachmentId { get; init; }
    }
}