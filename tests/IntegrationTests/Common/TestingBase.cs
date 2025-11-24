using Api.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace IntegrationTests.Common;

[Collection("TestingFixtureCollection")]
public abstract class TestingBase : IAsyncLifetime
{
    private readonly TestingFixture _fixture;
    private readonly IServiceScope _scope;

    protected TestingBase(TestingFixture fixture)
    {
        _fixture = fixture;
        _scope = _fixture.CreateScope();

        var dbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        if (!dbContext.Database.CanConnect())
        {
            throw new NpgsqlException("Cannot connect to the database");
        }
    }

    protected static CancellationToken CurrentCancellationToken => TestContext.Current.CancellationToken;

    public async ValueTask InitializeAsync()
    {
        await _fixture.SetUpAsync();
    }

    public ValueTask DisposeAsync()
    {
        _scope.Dispose();
        GC.SuppressFinalize(this);

        return ValueTask.CompletedTask;
    }

    protected HttpClient CreateClient()
    {
        return _fixture.CreateClient();
    }
}