using DotNet.Testcontainers.Containers;

namespace Taskly.WebApi.IntegrationTests.Infrastructure.ContainerHubs;

public abstract class ContainerHub<TContainer> : IAsyncDisposable where TContainer : DockerContainer
{
    private const int MaxRetryAttempts = 5;
    private DockerContainer _dockerContainer = null!;

    protected CancellationToken CurrentCancellationToken => TestContext.Current.CancellationToken;

    public string ConnectionString => _dockerContainer.GetConnectionString();

    public async ValueTask DisposeAsync()
    {
        await _dockerContainer.StopAsync(CurrentCancellationToken);
        await _dockerContainer.DisposeAsync();

        GC.SuppressFinalize(this);
    }

    protected async ValueTask BuildAndInitializeDockerContainerAsync()
    {
        _dockerContainer = BuildDockerContainer();

        var attempts = 0;
        while (attempts < MaxRetryAttempts)
        {
            try
            {
                await _dockerContainer.StartAsync(CurrentCancellationToken);
                attempts = MaxRetryAttempts; // Exit the loop if the container starts successfully
            }
            catch (Exception)
            {
                attempts++;
                if (attempts >= MaxRetryAttempts)
                {
                    throw;
                }

                await Task.Delay(TimeSpan.FromSeconds(5), CurrentCancellationToken);
            }
        }
    }

    protected abstract TContainer BuildDockerContainer();
}