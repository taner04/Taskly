using IntegrationTests.Common.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;

namespace IntegrationTests.Common;

[CollectionDefinition("TestingFixtureCollection")]
public class TestingFixtureCollection : ICollectionFixture<TestingFixture>;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public sealed class TestingFixture : IAsyncLifetime
{
    private readonly PostgresTestDatabase _postgresTestDatabase = new();
    private IServiceScopeFactory _serviceScopeFactory = null!;
    private WebApiFactory _webApiFactory = null!;
    private Auth0Service _auth0Service = null!;

    public async ValueTask InitializeAsync()
    {
        await _postgresTestDatabase.InitializeAsync();
        _webApiFactory = new WebApiFactory(_postgresTestDatabase.DbConnection);
        _serviceScopeFactory = _webApiFactory.Services.GetRequiredService<IServiceScopeFactory>();
        _auth0Service = new Auth0Service(InitConfiguration());
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
        
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _auth0Service.GetAccessToken());

        return client;
    }

    private static IConfiguration InitConfiguration()
    {
        return new ConfigurationBuilder()
                   .AddJsonFile("appsettings.integration.json", optional: false, reloadOnChange: false)
                   .Build();
    }
}