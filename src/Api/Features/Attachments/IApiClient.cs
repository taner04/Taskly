using Api.Features.Attachments.Endpoints;
using Refit;

// ReSharper disable CheckNamespace

namespace Api;

public partial interface IApiClient
{
    [Post(Routes.Attachments.CompleteUpload)]
    Task CompleteAttachmentUploadAsync(
        AttachmentId attachmentId,
        CompleteAttachmentUpload.Command.CommandBody body,
        CancellationToken cancellationToken = default);

    [Get(Routes.Attachments.CompleteUpload)]
    Task DownloadAttachmentAsync(
        AttachmentId attachmentId,
        CancellationToken cancellationToken = default);
}