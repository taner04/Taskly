using Api.Features.Attachments.Endpoints;
using Api.Features.Attachments.Models;
using Api.Features.Shared;
using Microsoft.AspNetCore.Mvc;
using Refit;

// ReSharper disable CheckNamespace

namespace Api.Client;

public partial interface IApiClient
{
    [Post(Routes.Attachments.CompleteUpload)]
    Task<HttpResponseMessage> CompleteAttachmentUploadAsync(
        [FromRoute] AttachmentId attachmentId,
        [Body] CompleteUpload.Command.CommandBody body,
        CancellationToken cancellationToken = default);

    [Get(Routes.Attachments.Download)]
    Task<HttpResponseMessage> DownloadAttachmentAsync(
        [FromRoute] AttachmentId attachmentId,
        CancellationToken cancellationToken = default);
}