using Api.Features.Users.Model;
using IntegrationTests.Factories;
using Microsoft.Testing.Platform.Builder;
using Npgsql;

namespace IntegrationTests.Infrastructure.TestContainers.Postgres;

public sealed class PostgresTestDatabase : IAsyncDisposable
{
    private readonly List<string> _dbTablesToClear = ["Todos", "Tags", "Attachments"];
    private readonly PostgresContainer _postgresContainer = new();
    private DbContextOptions<ApplicationDbContext> _dbContextOptions = null!;

    public DbConnection DbConnection => new NpgsqlConnection(_postgresContainer.ConnectionString);

    public async ValueTask DisposeAsync()
    {
        await _postgresContainer.DisposeAsync();
    }

    public async Task<UserId> InitializeContainerAsync(string auth0Id)
    {
        await _postgresContainer.InitializeAsync();

        _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(_postgresContainer.ConnectionString)
            .Options;

        await using var context = new ApplicationDbContext(_dbContextOptions);
        await context.Database.MigrateAsync(TestsContext.CurrentCancellationToken);
        
        return await InitUserAsync(auth0Id, context);
    }

    public async Task ResetContainerAsync()
    {
        await using var context = new ApplicationDbContext(_dbContextOptions);

        foreach (var sql in _dbTablesToClear.Select(tableName => $"Delete from \"{tableName}\""))
        {
            await context.Database.ExecuteSqlRawAsync(sql, TestsContext.CurrentCancellationToken);
        }
    }
    
    private async Task<UserId> InitUserAsync(string auth0Id, ApplicationDbContext context)
    {
        var user = UserFactory.Create(auth0Id);
        user.SetCreated(auth0Id);

        context.Users.Add(user);
        await context.SaveChangesAsync(TestsContext.CurrentCancellationToken);
        
        return  user.Id;
    }
}