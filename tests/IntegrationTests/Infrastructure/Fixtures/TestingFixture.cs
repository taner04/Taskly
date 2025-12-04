using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using IntegrationTests.Infrastructure.Azurite;

namespace IntegrationTests.Infrastructure.Fixtures;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public sealed class TestingFixture : IAsyncLifetime
{
    private readonly BlobTestStorage _blobTestStorage = new();
    private readonly PostgresTestDatabase _postgresTestDatabase = new();
    private string _jwtToken = null!;
    private IServiceScopeFactory _serviceScopeFactory = null!;
    private WebApiFactory _webApiFactory = null!;

    public async ValueTask InitializeAsync()
    {
        await _postgresTestDatabase.InitializeAsync();
        await _blobTestStorage.InitializeAsync();

        _webApiFactory = new WebApiFactory(_postgresTestDatabase.DbConnection, _blobTestStorage.ConnectionString);
        _serviceScopeFactory = _webApiFactory.Services.GetRequiredService<IServiceScopeFactory>();

        _jwtToken = await new Auth0Service(InitConfiguration()).GetAccessTokenAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _webApiFactory.DisposeAsync();
        await _postgresTestDatabase.DisposeAsync();
    }

    public async Task SetUpAsync()
    {
        await _postgresTestDatabase.ResetDatabaseAsync();
        await _blobTestStorage.ResetBlobStorageAsync();
    }

    public IServiceScope CreateScope()
    {
        return _serviceScopeFactory.CreateScope();
    }

    public HttpClient CreateAuthenticatedClient()
    {
        var client = _webApiFactory.CreateClient();

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _jwtToken);

        return client;
    }

    public HttpClient CreateUnauthenticatedClient()
    {
        return _webApiFactory.CreateClient();
    }

    private static IConfiguration InitConfiguration()
    {
        return new ConfigurationBuilder()
            .AddJsonFile("appsettings.integration.json", false, false)
            .Build();
    }
}