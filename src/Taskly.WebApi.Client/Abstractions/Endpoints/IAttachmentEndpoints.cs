using Microsoft.AspNetCore.Mvc;
using Taskly.WebApi.Features.Attachments.Endpoints;

namespace Taskly.WebApi.Client.Abstractions.Endpoints;

public interface IAttachmentEndpoints
{
    [Post(ApiRoutes.Attachments.CompleteUpload)]
    Task<HttpResponseMessage> CompleteAttachmentUploadAsync(
        [FromRoute] AttachmentId attachmentId,
        [Body] CompleteUpload.Command.CommandBody body,
        CancellationToken cancellationToken = default);

    [Get(ApiRoutes.Attachments.Download)]
    Task<HttpResponseMessage> DownloadAttachmentAsync(
        [FromRoute] AttachmentId attachmentId,
        CancellationToken cancellationToken = default);
}