using Testcontainers.PostgreSql;

namespace Taskly.IntegrationTests.Infrastructure.TestContainers.Postgres;

public sealed class PostgresContainer : ContainerBase<PostgreSqlContainer>
{
    public string ConnectionString => Container.GetConnectionString();

    protected override PostgreSqlContainer BuildContainer() =>
        new PostgreSqlBuilder("postgres:latest")
            .WithDatabase("db")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilCommandIsCompleted("pg_isready"))
            .WithCleanUp(true)
            .Build();
}
