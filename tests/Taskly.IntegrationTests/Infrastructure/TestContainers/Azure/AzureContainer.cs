using DotNet.Testcontainers.Containers;
using Testcontainers.Azurite;

namespace Taskly.IntegrationTests.Infrastructure.TestContainers.Azure;

public sealed class AzureContainer : ContainerHub
{
    public string ConnectionString => DockerContainer.GetConnectionString();

    protected override DockerContainer BuildContainer() =>
        new AzuriteBuilder("mcr.microsoft.com/azure-storage/azurite:latest")
            .WithCommand("--skipApiVersionCheck")
            .WithPortBinding(10000, 10000) // Blob service
            .WithWaitStrategy(
                Wait.ForUnixContainer()
                    .UntilMessageIsLogged("Azurite Blob service is starting at http://0.0.0.0:10000")
            )
            .WithCleanUp(true)
            .Build();
}