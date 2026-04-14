using DotNet.Testcontainers.Containers;
using Testcontainers.PostgreSql;

namespace Taskly.WebApi.IntegrationTests.Infrastructure.TestContainers.Postgres;

public sealed class PostgresContainer : ContainerHub
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