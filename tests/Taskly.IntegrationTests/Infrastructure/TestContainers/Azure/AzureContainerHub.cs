using Taskly.ServiceDefaults;

namespace Taskly.IntegrationTests.Infrastructure.TestContainers.Azure;

internal sealed class AzureContainerHub : IAsyncDisposable
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
        _blobContainerClient = blobServiceClient.GetBlobContainerClient(AppHostConstants.AzureBlobContainerName);

        await _blobContainerClient.CreateIfNotExistsAsync();
    }

    public async Task ResetContainerAsync()
    {
        await foreach (var blob in _blobContainerClient.GetBlobsAsync(
                           cancellationToken: TestContext.Current.CancellationToken))
        {
            var blobClient = _blobContainerClient.GetBlobClient(blob.Name);
            await blobClient.DeleteIfExistsAsync(cancellationToken: TestContext.Current.CancellationToken);
        }
    }
}