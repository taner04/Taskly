using System.Diagnostics.CodeAnalysis;
using IntegrationTests.Common.Database;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.Common;

[CollectionDefinition("TestingFixtureCollection")]
public class TestingFixtureCollection : ICollectionFixture<TestingFixture>;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public sealed class TestingFixture : IAsyncLifetime
{
    private readonly PostgresTestDatabase _postgresTestDatabase = new();
    private IServiceScopeFactory _serviceScopeFactory = null!;
    private WebApiFactory _webApiFactory = null!;

    public async ValueTask InitializeAsync()
    {
        await _postgresTestDatabase.InitializeAsync();
        _webApiFactory = new WebApiFactory(_postgresTestDatabase.DbConnection);
        _serviceScopeFactory = _webApiFactory.Services.GetRequiredService<IServiceScopeFactory>();
    }

    public async ValueTask DisposeAsync()
    {
        await _postgresTestDatabase.DisposeAsync();
        await _webApiFactory.DisposeAsync();
    }

    public async Task SetUpAsync()
    {
        await _postgresTestDatabase.ResetDatabaseAsync();
    }

    public IServiceScope CreateScope()
    {
        return _serviceScopeFactory.CreateScope();
    }

    public HttpClient CreateClient()
    {
        var client = _webApiFactory.CreateClient();
    
    
        return client;
    }
}