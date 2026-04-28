namespace Taskly.WebApi.Client.Common.Abstractions;

public interface IAttachmentService
{
    Task<WebClientResult<string>> DownloadAttachmentAsync(Guid attachmentId, CancellationToken cancellationToken);
}