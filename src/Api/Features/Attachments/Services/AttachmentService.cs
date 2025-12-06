using Azure.Storage.Blobs;
using Azure.Storage.Sas;

namespace Api.Features.Attachments.Services;

public sealed partial class AttachmentService(
    ILogger<AttachmentService> logger,
    BlobServiceClient blobServiceClient)
{
    private readonly BlobContainerClient _container =
        blobServiceClient.GetBlobContainerClient(Attachment.DefaultContainer);

    public async Task InitializeAsync()
    {
        await _container.CreateIfNotExistsAsync();
    }

    public SasDownloadResult GenerateDownloadSas(Attachment attachment)
    {
        var blob = _container.GetBlobClient(attachment.BlobName);

        if (!blob.CanGenerateSasUri)
        {
            LogDownloadSasNotAllowed(attachment.BlobName);
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

        LogDownloadSasGenerated(attachment.BlobName);

        return new SasDownloadResult(sasUri.ToString());
    }

    public SasUploadResult GenerateUploadSas(Attachment attachment)
    {
        var blobClient = _container.GetBlobClient(attachment.BlobName);

        if (!blobClient.CanGenerateSasUri)
        {
            LogUploadSasNotAllowed(attachment.BlobName);
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

        LogUploadSasGenerated(attachment.BlobName);

        return new SasUploadResult(sasUri.ToString(), attachment.BlobName);
    }

    public async Task<bool> DeleteAsync(Attachment attachment, CancellationToken ct)
    {
        try
        {
            var blob = _container.GetBlobClient(attachment.BlobName);
            var response = await blob.DeleteIfExistsAsync(cancellationToken: ct);

            if (response.Value)
                LogDeleteSucceeded(attachment.BlobName);
            else
                LogDeleteNotFound(attachment.BlobName);

            return response.Value;
        }
        catch (Exception ex)
        {
            LogFailedDelete(attachment.BlobName, ex);
            return false;
        }
    }

    [LoggerMessage(
        EventId = 0,
        Level = LogLevel.Warning,
        Message = "Cannot generate download SAS for blob {blob}: SAS not allowed.")]
    private partial void LogDownloadSasNotAllowed(string blob);

    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Information,
        Message = "Generated download SAS for blob {blob}.")]
    private partial void LogDownloadSasGenerated(string blob);


    [LoggerMessage(
        EventId = 2,
        Level = LogLevel.Warning,
        Message = "Cannot generate upload SAS for blob {blob}: SAS not allowed.")]
    private partial void LogUploadSasNotAllowed(string blob);

    [LoggerMessage(
        EventId = 3,
        Level = LogLevel.Information,
        Message = "Generated upload SAS for blob {blob}.")]
    private partial void LogUploadSasGenerated(string blob);


    [LoggerMessage(
        EventId = 4,
        Level = LogLevel.Information,
        Message = "Successfully deleted blob {blob}.")]
    private partial void LogDeleteSucceeded(string blob);

    [LoggerMessage(
        EventId = 5,
        Level = LogLevel.Warning,
        Message = "Blob {blob} not found during delete operation.")]
    private partial void LogDeleteNotFound(string blob);

    [LoggerMessage(
        EventId = 6,
        Level = LogLevel.Error,
        Message = "Failed to delete blob {blob}.")]
    private partial void LogFailedDelete(string blob, Exception exception);


    // Records
    public sealed record SasDownloadResult(string DownloadUrl);

    public sealed record SasUploadResult(string UploadUrl, string BlobPath);
}