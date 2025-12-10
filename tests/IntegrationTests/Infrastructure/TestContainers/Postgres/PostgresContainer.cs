using Testcontainers.PostgreSql;

namespace IntegrationTests.Infrastructure.TestContainers.Postgres;

public sealed class PostgresContainer : ContainerBase<PostgreSqlContainer>
{
    private const int MaxRetries = 5;

    public string ConnectionString => Container.GetConnectionString();

    protected override PostgreSqlContainer BuildContainer()
    {
        return new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .WithDatabase("db")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilCommandIsCompleted("pg_isready"))
            .WithCleanUp(true)
            .Build();
    }
}