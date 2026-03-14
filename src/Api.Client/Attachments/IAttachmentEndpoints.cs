using Api.Features.Attachments.Endpoints;
using Api.Features.Attachments.Models;
using Api.Features.Shared;
using Microsoft.AspNetCore.Mvc;
using Refit;

namespace Api.Client.Attachments;

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