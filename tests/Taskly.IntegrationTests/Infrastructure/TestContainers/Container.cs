using DotNet.Testcontainers.Containers;

namespace Taskly.IntegrationTests.Infrastructure.TestContainers;

public abstract class Container : IAsyncDisposable
{
    private const int MaxRetryAttempts = 5;
    protected DockerContainer DockerContainer = null!;

    public async ValueTask DisposeAsync()
    {
        await DockerContainer.StopAsync(TestContext.Current.CancellationToken);
        await DockerContainer.DisposeAsync();

        GC.SuppressFinalize(this);
    }

    public async ValueTask InitializeAsync()
    {
        DockerContainer = BuildContainer();

        var attempts = 0;
        while (attempts < MaxRetryAttempts)
        {
            try
            {
                await DockerContainer.StartAsync(TestContext.Current.CancellationToken);
                attempts = MaxRetryAttempts; // Exit the loop if the container starts successfully
            }
            catch (Exception)
            {
                attempts++;
                if (attempts >= MaxRetryAttempts)
                {
                    throw;
                }

                await Task.Delay(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);
            }
        }
    }

    protected abstract DockerContainer BuildContainer();
}