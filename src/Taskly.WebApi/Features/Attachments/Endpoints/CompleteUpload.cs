using Taskly.WebApi.Features.Attachments.Exceptions;

namespace Taskly.WebApi.Features.Attachments.Endpoints;

//TODO: migrate to event grid
[Handler]
[MapPost(ApiRoutes.Attachments.CompleteUpload)]
[Authorize(Policy = Policies.Roles.User)]
public static partial class CompleteUpload
{
    internal static void CustomizeEndpoint(
        IEndpointConventionBuilder endpoint)
    {
        endpoint.WithTags(nameof(Attachment));
    }

    private static async ValueTask HandleAsync(
        Command command,
        TasklyDbContext context,
        CurrentUserService currentUserService,
        CancellationToken ct)
    {
        InvalidAttachmentFileSizeException.ThrowInvalid(command.Body.FileSize);

        var userId = currentUserService.GetUserId();

        var attachment = await context.Attachments
                             .Include(a => a.Todo)
                             .SingleOrDefaultAsync(a => a.Id == command.AttachmentId
                                                        && a.Todo.UserId == userId, ct) ??
                         throw new ModelNotFoundException<Attachment>(command.AttachmentId.Value);

        if (!command.Body.IsUploaded)
        {
            context.Attachments.Remove(attachment);
        }
        else
        {
            attachment.FileSize = command.Body.FileSize;
            attachment.Status = AttachmentStatus.Uploaded;
            context.Update(attachment);
        }

        await context.SaveChangesAsync(ct);
    }

    [Validate]
    public sealed partial record Command : IValidationTarget<Command>
    {
        [FromRoute] public AttachmentId AttachmentId { get; init; }
        [NotNull] public required CommandBody Body { get; init; }

        [Validate]
        public sealed partial record CommandBody : IValidationTarget<CommandBody>
        {
            public required long FileSize { get; init; }
            public required bool IsUploaded { get; init; }
        }
    }
}