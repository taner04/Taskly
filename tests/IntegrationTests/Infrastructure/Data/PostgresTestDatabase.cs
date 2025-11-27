using System.Data.Common;
using Npgsql;

namespace IntegrationTests.Infrastructure.Data;

public sealed class PostgresTestDatabase : IAsyncDisposable
{
    private readonly PostgresContainer _postgresContainer = new();
    private string _connectionString = null!;
    private DbContextOptions<ApplicationDbContext> _dbContextOptions = null!;

    private List<string> _tableNames = [];

    public DbConnection DbConnection => new NpgsqlConnection(_postgresContainer.ConnectionString);

    public async ValueTask DisposeAsync()
    {
        await _postgresContainer.DisposeAsync();
    }

    public async Task InitializeAsync()
    {
        await _postgresContainer.InitializeAsync();

        var builder = new NpgsqlConnectionStringBuilder(_postgresContainer.ConnectionString);

        _connectionString = builder.ConnectionString;

        _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(_connectionString)
            .Options;

        await using var context = new ApplicationDbContext(_dbContextOptions);
        await context.Database.MigrateAsync();

        _tableNames = GetTableNames(context);
    }

    private static List<string> GetTableNames(ApplicationDbContext context)
    {
        var excludedTables = new HashSet<string>
        {
            "outboxevents",
            "__efmigrationshistory"
        };

        return context.Model.GetEntityTypes()
            .Select(t => t.GetTableName())
            .Where(name => name != null && !excludedTables.Contains(name.ToLower()))
            .Distinct()
            .ToList()!;
    }

    public async Task ResetDatabaseAsync()
    {
        await using var context = new ApplicationDbContext(_dbContextOptions);

        foreach (var sql in _tableNames.Select(tableName => $"Delete from \"{tableName}\""))
        {
            await context.Database.ExecuteSqlRawAsync(sql);
        }
    }
}