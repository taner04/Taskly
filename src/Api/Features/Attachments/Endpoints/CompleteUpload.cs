namespace Api.Features.Attachments.Endpoints;

[Handler]
[MapPost(Routes.Attachments.CompleteUpload)]
[Authorize(Policy = Policies.User)]
public static partial class CompleteUpload
{
    internal static void CustomizeEndpoint(
        IEndpointConventionBuilder endpoint)
    {
        endpoint.WithTags(nameof(Attachment));
    }

    private static async ValueTask HandleAsync(
        Command command,
        ApplicationDbContext context,
        CurrentUserService currentUserService,
        CancellationToken ct)
    {
        var userId = currentUserService.GetUserId();

        var attachment = await context.Attachments
            .Include(a => a.Todo)
            .SingleOrDefaultAsync(a => a.Id == command.AttachmentId
                                       && a.Todo.UserId == userId, ct);


        if (attachment is null)
        {
            throw new ModelNotFoundException<Attachment>(command.AttachmentId.Value);
        }

        if (!command.Body.IsUploaded)
        {
            context.Attachments.Remove(attachment);
        }
        else
        {
            attachment.MarkUploaded(command.Body.FileSize);
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