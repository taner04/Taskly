using Npgsql;
using Taskly.IntegrationTests.Factories;
using Taskly.WebApi.Common.Infrastructure.Persistence;
using Taskly.WebApi.Features.Users.Models;
using UserId = Taskly.WebApi.Features.Users.Models.UserId;

namespace Taskly.IntegrationTests.Infrastructure.TestContainers.Postgres;

public sealed class PostgresContainerHub : IAsyncDisposable
{
    private readonly PostgresContainer _postgresContainer = new();
    private DbContextOptions<TasklyDbContext> _dbContextOptions = null!;

    public DbConnection DbConnection => new NpgsqlConnection(_postgresContainer.ConnectionString);

    public async ValueTask DisposeAsync()
    {
        await _postgresContainer.DisposeAsync();
    }

    public async Task InitializeContainerAsync()
    {
        await _postgresContainer.InitializeAsync();

        _dbContextOptions = new DbContextOptionsBuilder<TasklyDbContext>()
            .UseNpgsql(_postgresContainer.ConnectionString)
            .Options;

        await using var context = new TasklyDbContext(_dbContextOptions);

        await context.Database.MigrateAsync(TestContext.Current.CancellationToken);

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

        await context.Database.ExecuteSqlRawAsync(sql, TestContext.Current.CancellationToken);

        await InitUserAsync(context);
    }

    public async Task<UserId> CreateForeignUserAsync()
    {
        await using var context = new TasklyDbContext(_dbContextOptions);

        var user = User.Create("otheruser@mail.com", "auth0|otheruserid123");
        user.SetCreated("auth0|otheruserid123");

        context.Users.Add(user);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        return user.Id;
    }

    private static async Task InitUserAsync(
        TasklyDbContext context)
    {
        var user = UserFactory.Create();
        user.SetCreated(UserFactory.Sub);

        context.Users.Add(user);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
    }
}