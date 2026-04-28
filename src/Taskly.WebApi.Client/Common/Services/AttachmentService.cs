using Taskly.Shared.WebApi.Responses.Attachments;

namespace Taskly.WebApi.Client.Common.Services;

public class AttachmentService(
    IHttpClientFactory httpClientFactory,
    IRefitWebApiClient webApiClient) : IAttachmentService
{
    public async Task<WebClientResult<string>> DownloadAttachmentAsync(
        Guid attachmentId,
        CancellationToken cancellationToken)
    {
        var downloadAttachmentResponse =
            await HttpOrchestrator.OrchestrateAsync<DownloadAttachmentResponse>(
                () => webApiClient.DownloadAttachmentAsync(attachmentId, cancellationToken),
                cancellationToken);

        if (!downloadAttachmentResponse.IsSuccess)
        {
            return downloadAttachmentResponse.Error;
        }

        using var httpClient = httpClientFactory.CreateClient();

        try
        {
            var contentBytes =
                await httpClient.GetByteArrayAsync(downloadAttachmentResponse.Value.DownloadUrl, cancellationToken);

            if (contentBytes.Length == 0)
            {
                return WebClientError.CustomError(
                    "Attachment.DownloadFailed",
                    "Failed to download attachment",
                    "Could not download the file from the cloud");
            }

            var documentsPath =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Taskly");
            if (!Directory.Exists(documentsPath))
            {
                Directory.CreateDirectory(documentsPath);
            }

            var filePath = Path.Combine(documentsPath, downloadAttachmentResponse.Value.FileName);
            await File.WriteAllBytesAsync(filePath, contentBytes, cancellationToken);

            return filePath;
        }
        catch (HttpRequestException ex)
        {
            return WebClientError.CustomError(
                "Attachment.DownloadFailed",
                "Failed to download attachment",
                $"Could not download from cloud: {ex.Message}");
        }
        catch (IOException ex)
        {
            return WebClientError.CustomError(
                "Attachment.SaveFailed",
                "Failed to save attachment",
                $"Could not write file: {ex.Message}");
        }
        catch (OperationCanceledException)
        {
            return WebClientError.CustomError(
                "Attachment.Cancelled",
                "Download cancelled",
                "The operation was cancelled");
        }
        catch (Exception ex)
        {
            return WebClientError.CustomError(
                "Attachment.UnexpectedError",
                "Unexpected error while downloading attachment",
                ex.Message);
        }
    }
}