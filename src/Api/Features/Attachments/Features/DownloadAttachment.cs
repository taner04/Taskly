using Api.Features.Attachments.Models;
using Api.Features.Attachments.Services;
using Api.Features.Users;
using Api.Shared.Features.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.Attachments.Features;

[Handler]
[MapGet(Routes.Attachments.Download)]
[Authorize]
public static partial class DownloadAttachment
{
    internal static void CustomizeEndpoint(
        IEndpointConventionBuilder endpoint)
    {
        endpoint.WithTags(nameof(Attachment));
    }

    internal static Results<FileStreamHttpResult, NotFound<Error>> TransformResult(
        ErrorOr<FileStreamHttpResult> result)
    {
        return result.Match<
            Results<FileStreamHttpResult, NotFound<Error>>>
        (
            _ => result.Value,
            error => TypedResults.NotFound(error.First())
        );
    }

    private static async ValueTask<ErrorOr<FileStreamHttpResult>> HandleAsync(
        Query query,
        ApplicationDbContext context,
        CurrentUserService currentUserService,
        AttachmentService attachmentService,
        CancellationToken ct)
    {
        var userId = currentUserService.GetCurrentUserId();

        var todo = await context.Todos
            .Include(t => t.Attachments)
            .SingleOrDefaultAsync(
                t => t.Attachments.Any(a => a.Id == query.AttachmentId)
                     && t.UserId == userId,
                ct);

        if (todo is null)
        {
            return Error.NotFound("Attachment.NotFound",
                "The attachment does not exist or you do not have permission.");
        }

        var attachment = todo.Attachments
            .Single(a => a.Id == query.AttachmentId);

        var stream = await attachmentService.DownloadAsync(attachment, ct);

        if (stream is null)
        {
            return Error.NotFound("Blob.NotFound",
                "The stored attachment file could not be found.");
        }

        var result = TypedResults.File(
            stream,
            attachment.ContentType,
            attachment.FileName
        );

        return result;
    }

    [Validate]
    public sealed partial record Query : IValidationTarget<Query>
    {
        [NotEmpty] [FromRoute] public required AttachmentId AttachmentId { get; init; }
    }
}