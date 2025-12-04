using Api.Features.Attachments.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;

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

    public SasDownloadResult GenerateDownloadSas(
        Attachment attachment)
    {
        var blob = _container.GetBlobClient(attachment.BlobName);

        if (!blob.CanGenerateSasUri)
        {
            throw new InvalidOperationException("SAS generation not allowed.");
        }

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = attachment.Container,
            BlobName = attachment.BlobName,
            Resource = "b",
            ExpiresOn = DateTime.UtcNow.AddMinutes(5)
        };

        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        var sasUri = blob.GenerateSasUri(sasBuilder);

        return new SasDownloadResult(sasUri.ToString());
    }

    public SasUploadResult GenerateUploadSas(
        Attachment attachment)
    {
        var blobClient = _container.GetBlobClient(attachment.BlobName);

        if (!blobClient.CanGenerateSasUri)
        {
            throw new InvalidOperationException(
                "Cannot generate SAS URL. Ensure BlobServiceClient is created with a key credential.");
        }

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = _container.Name,
            BlobName = attachment.BlobName,
            Resource = "blob",
            ExpiresOn = DateTime.UtcNow.AddMinutes(10)
        };

        sasBuilder.SetPermissions(BlobSasPermissions.Create | BlobSasPermissions.Write);

        var sasUri = blobClient.GenerateSasUri(sasBuilder);

        return new SasUploadResult(
            sasUri.ToString(),
            attachment.BlobName
        );
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
            logger.LogError(ex, "Failed to delete blob {Blob}", attachment.BlobName);
            return false;
        }
    }

    public sealed record SasDownloadResult(string DownloadUrl);

    public sealed record SasUploadResult(string UploadUrl, string BlobPath);
}