using Taskly.WebApi.Features.Attachments.WebHooks;

namespace Taskly.WebApi.Client.Common.Refit.Endpoints;

public interface IAttachmentEndpoints
{
    [Get(ApiRoutes.Attachments.Download)]
    Task<HttpResponseMessage> DownloadAttachmentAsync(
        Guid attachmentId,
        CancellationToken cancellationToken = default);

    [Post(ApiRoutes.Attachments.WebHook)]
    Task<HttpResponseMessage> SendAttachmentEventAsync(
        [Body] AttachmentEventData command,
        [Header(AttachmentWebHookConstants.RequestHeader)]
        string webHookSecret,
        CancellationToken cancellationToken = default);
}