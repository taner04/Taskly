using DotNet.Testcontainers.Containers;

namespace IntegrationTests.Infrastructure;

public abstract class ContainerBase<T> : IAsyncLifetime where T : DockerContainer
{
    private const int MaxRetries = 5;
    protected T Container = null!;

    public async ValueTask DisposeAsync()
    {
        await Container.StopAsync();
        await Container.DisposeAsync();

        GC.SuppressFinalize(this);
    }

    public async ValueTask InitializeAsync()
    {
        await StartWithRetryAsync();
    }

    protected abstract T BuildContainer();

    private async ValueTask StartWithRetryAsync()
    {
        Container = BuildContainer();

        var attempt = 0;
        while (attempt < MaxRetries)
        {
            try
            {
                await Container.StartAsync();
                attempt = MaxRetries;
            }
            catch (Exception ex)
            {
                attempt++;
                if (attempt >= MaxRetries)
                {
                    throw new Exception($"Failed to start PostgresSQL container after {MaxRetries} attempts.", ex);
                }

                await Task.Delay(2000);
            }
        }
    }
}