using Microsoft.AspNetCore.Mvc;
using Taskly.WebApi.Features.Attachments.WebHooks;

namespace Taskly.WebApi.Client.Abstractions.Endpoints;

public interface IAttachmentEndpoints
{
    [Get(ApiRoutes.Attachments.Download)]
    Task<HttpResponseMessage> DownloadAttachmentAsync(
        [FromRoute] AttachmentId attachmentId,
        CancellationToken cancellationToken = default);

    [Post(ApiRoutes.Attachments.WebHook)]
    Task<HttpResponseMessage> ReceiveAttachmentWebHookAsync(
        [Body] AttachmentEventWebHook.Command command,
        CancellationToken cancellationToken = default);
}