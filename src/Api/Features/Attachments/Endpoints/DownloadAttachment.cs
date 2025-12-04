using Api.Features.Attachments.Exceptions;
using Api.Features.Attachments.Models;
using Api.Features.Attachments.Services;
using Api.Features.Users.Services;
using Api.Shared.Api;
using Microsoft.AspNetCore.Authorization;

namespace Api.Features.Attachments.Endpoints;

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

    private static async ValueTask<ErrorOr<Dto>> HandleAsync(
        Query query,
        ApplicationDbContext db,
        CurrentUserService current,
        AttachmentService attachments,
        CancellationToken ct)
    {
        var userId = current.GetCurrentUserId();

        var attachment = await db.Attachments
            .Include(a => a.Todo)
            .SingleOrDefaultAsync(a =>
                    a.Id == query.AttachmentId &&
                    a.Todo.UserId == userId,
                ct);

        if (attachment is null)
        {
            throw new AttachmentNotFoundException(query.AttachmentId);
        }

        var sas = attachments.GenerateDownloadSas(attachment);

        return new Dto(
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

    public sealed record Dto(
        string DownloadUrl,
        string FileName,
        string ContentType);
}