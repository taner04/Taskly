using Api.Features.Attachments.Models;

namespace IntegrationTests.Infrastructure.Azurite;

internal sealed class BlobTestStorage : IAsyncDisposable
{
    private readonly BlobContainer _blobContainer = new();
    private BlobContainerClient _blobContainerClient = null!;

    public string ConnectionString => _blobContainer.ConnectionString;

    public async ValueTask DisposeAsync()
    {
        await _blobContainer.DisposeAsync();
    }

    public async Task InitializeAsync()
    {
        await _blobContainer.InitializeAsync();

        var blobServiceClient = new BlobServiceClient(_blobContainer.ConnectionString);

        _blobContainerClient = blobServiceClient.GetBlobContainerClient(Attachment.DefaultContainer);
        if (!await _blobContainerClient.ExistsAsync())
        {
            await _blobContainerClient.CreateAsync();
        }
    }

    public async Task ResetBlobStorageAsync()
    {
        await foreach (var blob in _blobContainerClient.GetBlobsAsync(
                           cancellationToken: _blobContainer.CurrentCancellationToken))
        {
            var blobClient = _blobContainerClient.GetBlobClient(blob.Name);
            await blobClient.DeleteIfExistsAsync(cancellationToken: _blobContainer.CurrentCancellationToken);
        }
    }
}