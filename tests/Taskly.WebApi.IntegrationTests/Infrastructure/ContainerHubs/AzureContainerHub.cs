using Taskly.ServiceDefaults;
using Testcontainers.Azurite;

namespace Taskly.WebApi.IntegrationTests.Infrastructure.ContainerHubs;

internal sealed class AzureContainerHub : ContainerHub<AzuriteContainer>
{
    private BlobContainerClient _blobContainerClient = null!;

    protected override AzuriteContainer BuildDockerContainer() =>
        new AzuriteBuilder("mcr.microsoft.com/azure-storage/azurite:latest")
            .WithCommand("--skipApiVersionCheck")
            .WithPortBinding(10000, 10000) // Blob service
            .WithWaitStrategy(
                Wait.ForUnixContainer()
                    .UntilMessageIsLogged("Azurite Blob service is starting at http://0.0.0.0:10000")
            )
            .WithCleanUp(true)
            .Build();

    public async Task InitializeContainerAsync()
    {
        await BuildAndInitializeDockerContainerAsync();

        var blobServiceClient = new BlobServiceClient(ConnectionString);
        _blobContainerClient = blobServiceClient.GetBlobContainerClient(AppHostConstants.AzureBlobContainerName);

        await _blobContainerClient.CreateIfNotExistsAsync();
    }

    public async Task ResetContainerAsync()
    {
        await foreach (var blob in _blobContainerClient.GetBlobsAsync(cancellationToken: CurrentCancellationToken))
        {
            var blobClient = _blobContainerClient.GetBlobClient(blob.Name);
            await blobClient.DeleteIfExistsAsync(cancellationToken: CurrentCancellationToken);
        }
    }
}