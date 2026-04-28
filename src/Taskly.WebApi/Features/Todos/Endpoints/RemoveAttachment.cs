using Taskly.WebApi.Features.Attachments.Common.Exceptions;
using Taskly.WebApi.Features.Todos.Common.Extensions;
using Taskly.WebApi.Features.Todos.Common.Specifications;
using AttachmentBlobContainerService =
    Taskly.WebApi.Features.Attachments.Common.Services.AttachmentBlobContainerService;
using AttachmentId = Taskly.WebApi.Features.Attachments.Common.Models.AttachmentId;

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

    internal static Ok<GetTodoResponse> TransformResult(
        GetTodoResponse response) =>
        TypedResults.Ok(response);

    private static async ValueTask<GetTodoResponse> HandleAsync(
        [AsParameters] Command command,
        TasklyDbContext context,
        CurrentUserService currentUserService,
        AttachmentBlobContainerService attachmentBlobContainerService,
        CancellationToken ct)
    {
        var userId = currentUserService.GetUserId();

        var spec = new TodoByUserIdSpecification(command.TodoId, userId);
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

            return todo.ToGetTodoResponse();
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