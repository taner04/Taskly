using Api.Features.Attachments.Endpoints;
using Refit;

// ReSharper disable CheckNamespace

namespace Api;

public partial interface IApiClient
{
    [Post(Routes.Attachments.CompleteUpload)]
    Task<HttpResponseMessage> CompleteAttachmentUploadAsync(
        [FromRoute] AttachmentId attachmentId,
        [Body] CompleteAttachmentUpload.Command.CommandBody body,
        CancellationToken cancellationToken = default);

    [Get(Routes.Attachments.Download)]
    Task<HttpResponseMessage> DownloadAttachmentAsync(
        [FromRoute] AttachmentId attachmentId,
        CancellationToken cancellationToken = default);
}