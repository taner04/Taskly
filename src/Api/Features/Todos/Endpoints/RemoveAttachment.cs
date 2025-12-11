using Api.Features.Attachments.Services;
using Api.Features.Todos.Exceptions;
using Api.Features.Todos.Specifications;
using Ardalis.Specification.EntityFrameworkCore;

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

    private static async ValueTask HandleAsync(
        Command command,
        ApplicationDbContext context,
        CurrentUserService currentUserService,
        AttachmentService attachments,
        CancellationToken ct)
    {
        var userId = currentUserService.GetUserId();
        
        var spec = new TodoByUserIdSpecificationWithAttachmentsSpec(command.TodoId, userId);
        var todo = await context.Todos
            .WithSpecification(spec)
            .SingleOrDefaultAsync(ct);
        
        if (todo is null)
        {
            throw new ModelNotFoundException<Todo>(command.TodoId.Value);
        }

        var attachment = todo.Attachments.SingleOrDefault(a => a.Id == command.AttachmentId);

        if (attachment is null)
        {
            throw new ModelNotFoundException<Attachment>(command.AttachmentId.Value);
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