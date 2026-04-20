using Taskly.WebApi.Features.Attachments.Services;

namespace Taskly.WebApi.Features.Attachments.Endpoints;

[Handler]
[MapGet(ApiRoutes.Attachments.Download)]
[Authorize(Policy = Security.Policies.User)]
public static partial class DownloadAttachment
{
    internal static void CustomizeEndpoint(
        RouteHandlerBuilder endpoint)
    {
        endpoint.WithTags(nameof(Attachment));
        endpoint.RequireRateLimiting(Security.RateLimiting.Global);
    }

    private static async ValueTask<Response> HandleAsync(
        Query query,
        TasklyDbContext db,
        CurrentUserService current,
        AttachmentBlobContainerService attachmentBlobContainerService,
        CancellationToken ct)
    {
        var userId = current.GetUserId();

        var attachment = await db.Attachments
            .Include(a => a.Todo)
            .SingleOrDefaultAsync(a =>
                    a.Id == query.AttachmentId &&
                    a.Todo.UserId == userId,
                ct) ?? throw new ModelNotFoundException<Attachment>(query.AttachmentId.Value);

        var sas = attachmentBlobContainerService.GenerateDownloadSas(attachment);

        return new Response(
            sas.DownloadUrl,
            attachment.FileName
        );
    }

    public sealed record Response(
        string DownloadUrl,
        string FileName);

    [Validate]
    public sealed partial record Query : IValidationTarget<Query>
    {
        [FromRoute] public required AttachmentId AttachmentId { get; init; }
    }
}