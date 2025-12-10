using Api.Features.Attachments.Services;

namespace Api.Features.Attachments.Endpoints;

[Handler]
[MapGet(Routes.Attachments.Download)]
[Authorize]
public static partial class Download
{
    internal static void CustomizeEndpoint(
        IEndpointConventionBuilder endpoint)
    {
        endpoint.WithTags(nameof(Attachment));
    }

    private static async ValueTask<Response> HandleAsync(
        Query query,
        ApplicationDbContext db,
        CurrentUserService current,
        AttachmentService attachments,
        CancellationToken ct)
    {
        var userId = current.GetUserId();

        var attachment = await db.Attachments
            .Include(a => a.Todo)
            .SingleOrDefaultAsync(a =>
                    a.Id == query.AttachmentId &&
                    a.Todo.UserId == userId,
                ct);

        if (attachment is null)
        {
            throw new ModelNotFoundException<Attachment>(query.AttachmentId.Value);
        }

        var sas = attachments.GenerateDownloadSas(attachment);

        return new Response(
            sas.DownloadUrl,
            attachment.FileName,
            attachment.ContentType
        );
    }


    [Validate]
    public sealed partial record Query : IValidationTarget<Query>
    {
        public required AttachmentId AttachmentId { get; init; }
    }

    public sealed record Response(
        string DownloadUrl,
        string FileName,
        string ContentType);
}