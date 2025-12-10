using DotNet.Testcontainers.Containers;

namespace IntegrationTests.Infrastructure.TestContainers;

public abstract class ContainerBase<T> : IAsyncLifetime where T : DockerContainer
{
    private const int MaxRetries = 5;

    protected T Container = null!;

    public async ValueTask InitializeAsync()
    {
        Container = BuildContainer();

        var attempt = 0;
        while (attempt < MaxRetries)
        {
            try
            {
                await Container.StartAsync(TestsContext.CurrentCancellationToken);
                attempt = MaxRetries;
            }
            catch (Exception ex)
            {
                attempt++;
                if (attempt >= MaxRetries)
                {
                    throw new Exception($"Failed to start PostgresSQL container after {MaxRetries} attempts.", ex);
                }

                await Task.Delay(2000, TestsContext.CurrentCancellationToken);
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        await Container.StopAsync(TestsContext.CurrentCancellationToken);
        await Container.DisposeAsync();

        GC.SuppressFinalize(this);
    }

    protected abstract T BuildContainer();
}