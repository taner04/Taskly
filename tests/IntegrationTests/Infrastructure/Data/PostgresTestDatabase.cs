using Npgsql;

namespace IntegrationTests.Infrastructure.Data;

public sealed class PostgresTestDatabase : IAsyncDisposable
{
    private readonly List<string> _dbTablesToClear = ["Todos", "Tags"];
    private readonly PostgresContainer _postgresContainer = new();
    private DbContextOptions<ApplicationDbContext> _dbContextOptions = null!;

    public DbConnection DbConnection => new NpgsqlConnection(_postgresContainer.ConnectionString);

    public async ValueTask DisposeAsync()
    {
        await _postgresContainer.DisposeAsync();
    }

    public async Task InitializeAsync()
    {
        await _postgresContainer.InitializeAsync();

        _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(_postgresContainer.ConnectionString)
            .Options;

        await using var context = new ApplicationDbContext(_dbContextOptions);
        await context.Database.MigrateAsync(_postgresContainer.CurrentCancellationToken);
    }

    public async Task ResetDatabaseAsync()
    {
        await using var context = new ApplicationDbContext(_dbContextOptions);

        foreach (var sql in _dbTablesToClear.Select(tableName => $"Delete from \"{tableName}\""))
        {
            await context.Database.ExecuteSqlRawAsync(sql, _postgresContainer.CurrentCancellationToken);
        }
    }
}