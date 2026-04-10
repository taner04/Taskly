using Taskly.WebApi.Common.Infrastructure.Persistence;
using Taskly.WebApi.Common.Shared;
using Taskly.WebApi.Common.Shared.Exceptions;
using Taskly.WebApi.Features.Attachments.Services;

namespace Taskly.WebApi.Features.Attachments.Endpoints;

[Handler]
[MapGet(ApiRoutes.Attachments.Download)]
[Authorize(Policy = Policies.Roles.User)]
public static partial class Download
{
    internal static void CustomizeEndpoint(
        IEndpointConventionBuilder endpoint)
    {
        endpoint.WithTags(nameof(Attachment));
        endpoint.RequireRateLimiting(Policies.RateLimiting.Global);
    }

    private static async ValueTask<Response> HandleAsync(
        Query query,
        TasklyDbContext db,
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
                ct) ?? throw new ModelNotFoundException<Attachment>(query.AttachmentId.Value);

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