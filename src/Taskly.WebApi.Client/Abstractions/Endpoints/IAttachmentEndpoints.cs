using Microsoft.AspNetCore.Mvc;
using Refit;
using Taskly.WebApi.Common.Shared;
using Taskly.WebApi.Features.Attachments.Endpoints;
using Taskly.WebApi.Features.Attachments.Models;

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