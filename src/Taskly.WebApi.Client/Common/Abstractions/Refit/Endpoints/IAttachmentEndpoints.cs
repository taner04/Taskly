using Taskly.WebApi.Features.Attachments.WebHooks;

namespace Taskly.WebApi.Client.Common.Abstractions.Refit.Endpoints;

public interface IAttachmentEndpoints
{
    [Get(ApiRoutes.Attachments.Download)]
    Task<HttpResponseMessage> DownloadAttachmentAsync(
        Guid attachmentId,
        CancellationToken cancellationToken = default);

    [Post(ApiRoutes.Attachments.WebHook)]
    Task<HttpResponseMessage> SendAttachmentEventAsync(
        [Body] AttachmentEventData command,
        CancellationToken cancellationToken = default);
}