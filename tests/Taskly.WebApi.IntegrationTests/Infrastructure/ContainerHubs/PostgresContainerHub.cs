using Taskly.WebApi.Common.Infrastructure.Persistence;
using Testcontainers.PostgreSql;

namespace Taskly.WebApi.IntegrationTests.Infrastructure.ContainerHubs;

public sealed class PostgresContainerHub : ContainerHub<PostgreSqlContainer>
{
    private DbContextOptions<TasklyDbContext> _dbContextOptions = null!;

    protected override PostgreSqlContainer BuildDockerContainer() =>
        new PostgreSqlBuilder("postgres:latest")
            .WithDatabase("db")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilCommandIsCompleted("pg_isready"))
            .WithCleanUp(true)
            .Build();

    public async Task InitializeContainerAsync()
    {
        await BuildAndInitializeDockerContainerAsync();

        _dbContextOptions = new DbContextOptionsBuilder<TasklyDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        await using var context = new TasklyDbContext(_dbContextOptions);

        await context.Database.MigrateAsync(CurrentCancellationToken);

        await InitUserAsync(context);
    }

    public async Task ResetContainerAsync()
    {
        await using var context = new TasklyDbContext(_dbContextOptions);

        const string sql = """
                           DELETE FROM "Todos";
                           DELETE FROM "Tags";
                           DELETE FROM "Attachments";
                           DELETE FROM "Users";
                           """;

        await context.Database.ExecuteSqlRawAsync(sql, CurrentCancellationToken);

        await InitUserAsync(context);
    }

    public async Task<UserId> CreateForeignUserAsync()
    {
        await using var context = new TasklyDbContext(_dbContextOptions);

        var user = User.Create("otheruser@mail.com", "auth0|otheruserid123");
        user.SetCreated("auth0|otheruserid123");

        context.Users.Add(user);
        await context.SaveChangesAsync(CurrentCancellationToken);

        return user.Id;
    }

    private async Task InitUserAsync(
        TasklyDbContext context)
    {
        var user = UserFactory.Create();
        user.SetCreated(UserFactory.Sub);

        context.Users.Add(user);
        await context.SaveChangesAsync(CurrentCancellationToken);
    }
}