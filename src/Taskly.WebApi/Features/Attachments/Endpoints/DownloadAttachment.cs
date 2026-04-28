using Taskly.Shared.WebApi.Responses.Attachments;
using AttachmentBlobContainerService =
    Taskly.WebApi.Features.Attachments.Common.Services.AttachmentBlobContainerService;
using AttachmentId = Taskly.WebApi.Features.Attachments.Common.Models.AttachmentId;

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

    private static async ValueTask<DownloadAttachmentResponse> HandleAsync(
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

        return new DownloadAttachmentResponse(
            sas.DownloadUrl,
            attachment.FileName
        );
    }

    [Validate]
    public sealed partial record Query : IValidationTarget<Query>
    {
        [FromRoute] public required AttachmentId AttachmentId { get; init; }
    }
}