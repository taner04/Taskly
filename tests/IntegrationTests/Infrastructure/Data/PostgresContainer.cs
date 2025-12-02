using DotNet.Testcontainers.Builders;
using Testcontainers.PostgreSql;

namespace IntegrationTests.Infrastructure.Data;

public sealed class PostgresContainer : IAsyncDisposable
{
    private const int MaxRetries = 5;

    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("db")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .WithWaitStrategy(Wait.ForUnixContainer().UntilCommandIsCompleted("pg_isready"))
        .WithCleanUp(true)
        .Build();

    public string ConnectionString => _container.GetConnectionString();

    public async ValueTask DisposeAsync()
    {
        await _container.StopAsync();
        await _container.DisposeAsync();
    }

    public async Task InitializeAsync()
    {
        await StartWithRetry();
    }

    private async Task StartWithRetry()
    {
        var attempt = 0;
        while (attempt < MaxRetries)
        {
            try
            {
                await _container.StartAsync();
                attempt = MaxRetries; // Exit loop on success
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