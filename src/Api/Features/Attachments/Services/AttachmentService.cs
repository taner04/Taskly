using Api.Features.Attachments.Models;
using Azure;
using Azure.Storage.Blobs;

namespace Api.Features.Attachments.Services;

public sealed class AttachmentService(
    ILogger<AttachmentService> logger,
    BlobServiceClient blobServiceClient)
{
    private readonly BlobContainerClient _container =
        blobServiceClient.GetBlobContainerClient(Attachment.DefaultContainer);

    public async Task InitializeAsync()
    {
        await _container.CreateIfNotExistsAsync();
    }

    public async Task<Stream?> DownloadAsync(
        Attachment attachment,
        CancellationToken ct)
    {
        var blob = _container.GetBlobClient(attachment.BlobName);

        try
        {
            if (!await blob.ExistsAsync(ct))
            {
                return null;
            }

            var response = await blob.DownloadAsync(ct);

            return response.Value.Content;
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            logger.LogError(ex, "Attachment {AttachmentBlobName} not found", attachment.BlobName);
            return null;
        }
        catch (RequestFailedException ex)
        {
            logger.LogError(ex, "Azure request failure during blob download");
            return null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during blob download");
            return null;
        }
    }


    public async Task<bool> UploadAsync(
        IFormFile file,
        Attachment attachment,
        CancellationToken ct)
    {
        try
        {
            var blob = _container.GetBlobClient(attachment.BlobName);

            await using var stream = file.OpenReadStream();

            await blob.UploadAsync(stream, false, ct);

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex,"Failed to upload attachment {AttachmentBlobName}", attachment.BlobName);
            return false;
        }
    }

    public async Task<bool> DeleteAsync(
        Attachment attachment,
        CancellationToken ct)
    {
        try
        {
            var blob = _container.GetBlobClient(attachment.BlobName);

            var response = await blob.DeleteIfExistsAsync(cancellationToken: ct);

            return response.Value;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Attachment {AttachmentBlobName} not found", attachment.BlobName);
            return false;
        }
    }
}