using DotNet.Testcontainers.Containers;
using Testcontainers.PostgreSql;

namespace Taskly.IntegrationTests.Infrastructure.TestContainers.Postgres;

public sealed class PostgresContainer : Container
{
    public string ConnectionString => DockerContainer.GetConnectionString();

    protected override DockerContainer BuildContainer() =>
        new PostgreSqlBuilder("postgres:latest")
            .WithDatabase("db")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilCommandIsCompleted("pg_isready"))
            .WithCleanUp(true)
            .Build();
}