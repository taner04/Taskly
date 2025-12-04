using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using Api;
using IntegrationTests.Infrastructure.TestContainers.Azure;
using IntegrationTests.Infrastructure.TestContainers.Postgres;
using Refit;

namespace IntegrationTests.Infrastructure.Fixtures;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public sealed class TestingFixture : IAsyncLifetime
{
    private readonly AzureTestBlobStorage _azureTestBlobStorage = new();
    private readonly PostgresTestDatabase _postgresTestDatabase = new();
    private ClaimsPrincipal _currentUser = null!;
    private string _jwtToken = null!;
    private IServiceScopeFactory _serviceScopeFactory = null!;
    private WebApiFactory _webApiFactory = null!;

    public async ValueTask InitializeAsync()
    {
        await _postgresTestDatabase.InitializeContainerAsync();
        await _azureTestBlobStorage.InitializeContainerAsync();

        _webApiFactory = new WebApiFactory(_postgresTestDatabase.DbConnection, _azureTestBlobStorage.ConnectionString);
        _serviceScopeFactory = _webApiFactory.Services.GetRequiredService<IServiceScopeFactory>();

        _jwtToken = await new Auth0Service(InitConfiguration()).GetAccessTokenAsync();

        var token = new JwtSecurityTokenHandler().ReadJwtToken(_jwtToken);
        _currentUser = new ClaimsPrincipal(new ClaimsIdentity(token.Claims));
    }

    public async ValueTask DisposeAsync()
    {
        await _webApiFactory.DisposeAsync();
        await _postgresTestDatabase.DisposeAsync();
    }

    public async Task SetUpAsync()
    {
        await _postgresTestDatabase.ResetContainerAsync();
        await _azureTestBlobStorage.ResetContainerAsync();
    }

    public IServiceScope CreateScope()
    {
        return _serviceScopeFactory.CreateScope();
    }

    public IApiClient CreateAuthenticatedClient()
    {
        var client = _webApiFactory.CreateClient();

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _jwtToken);

        return RestService.For<IApiClient>(client);
    }

    public IApiClient CreateUnauthenticatedClient()
    {
        return RestService.For<IApiClient>(_webApiFactory.CreateClient());
    }

    private static IConfiguration InitConfiguration()
    {
        return new ConfigurationBuilder()
            .AddJsonFile("appsettings.integration.json", false, false)
            .Build();
    }

    public string GetCurrentUserId()
    {
        var userId = _currentUser.FindFirst("azp")?.Value;

        return string.IsNullOrEmpty(userId)
            ? throw new UnauthorizedAccessException("UserId claim is missing.")
            : userId;
    }
}