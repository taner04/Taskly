using Api.Features.Attachments.Models;

namespace IntegrationTests.Infrastructure.TestContainers.Azure;

internal sealed class AzureTestBlobStorage : IAsyncDisposable
{
    private readonly AzureContainer _azureContainer = new();
    private BlobContainerClient _blobContainerClient = null!;

    public string ConnectionString => _azureContainer.ConnectionString;

    public async ValueTask DisposeAsync()
    {
        await _azureContainer.DisposeAsync();
    }

    public async Task InitializeContainerAsync()
    {
        await _azureContainer.InitializeAsync();

        var blobServiceClient = new BlobServiceClient(_azureContainer.ConnectionString);

        _blobContainerClient = blobServiceClient.GetBlobContainerClient(Attachment.DefaultContainer);
        if (!await _blobContainerClient.ExistsAsync()) await _blobContainerClient.CreateAsync();
    }

    public async Task ResetContainerAsync()
    {
        await foreach (var blob in _blobContainerClient.GetBlobsAsync(
                           cancellationToken: _azureContainer.CurrentCancellationToken))
        {
            var blobClient = _blobContainerClient.GetBlobClient(blob.Name);
            await blobClient.DeleteIfExistsAsync(cancellationToken: _azureContainer.CurrentCancellationToken);
        }
    }
}