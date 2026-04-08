using System.Diagnostics.CodeAnalysis;
using Taskly.IntegrationTests.Factories;
using Taskly.IntegrationTests.Infrastructure.Composition.Mocks.Jwt;
using Taskly.IntegrationTests.Infrastructure.TestContainers.Azure;
using Taskly.IntegrationTests.Infrastructure.TestContainers.Postgres;
using Taskly.WebApi.Client.Abstractions;
using Taskly.WebApi.Client.Common;

namespace Taskly.IntegrationTests.Infrastructure.Fixtures;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public sealed class TestingFixture : IAsyncLifetime
{
    private readonly AzureTestBlobStorage _azureTestBlobStorage = new();

    private readonly PostgresTestDatabase _postgresTestDatabase = new();

    private string _adminJwtToken = null!;
    private ApiHttpClient _apiHttpClient = null!;
    private IServiceScopeFactory _serviceScopeFactory = null!;
    private string _userJwtToken = null!;
    private WebApiFactory _webApiFactory = null!;

    public async ValueTask InitializeAsync()
    {
        InitilizeTokens();

        await _postgresTestDatabase.InitializeContainerAsync();

        await _azureTestBlobStorage.InitializeContainerAsync();

        _webApiFactory = new WebApiFactory(_postgresTestDatabase.DbConnection, _azureTestBlobStorage.ConnectionString);
        _serviceScopeFactory = _webApiFactory.Services.GetRequiredService<IServiceScopeFactory>();

        _apiHttpClient = new ApiHttpClient(_webApiFactory.CreateClient());
    }

    public async ValueTask DisposeAsync()
    {
        await _webApiFactory.DisposeAsync();
        await _postgresTestDatabase.DisposeAsync();
    }

    public async Task SetUpAsync()
    {
        _apiHttpClient?.ClearAccessToken();

        await _postgresTestDatabase.ResetContainerAsync();
        await _azureTestBlobStorage.ResetContainerAsync();
    }

    public async Task<UserId> CreateForeignUserAsync() => await _postgresTestDatabase.CreateForeignUserAsync();

    public IServiceScope CreateScope() => _serviceScopeFactory.CreateScope();

    public IApiClient CreateAuthenticatedClient(
        UserRole userRole)
    {
        _apiHttpClient.SetAccessToken(userRole switch
        {
            UserRole.User => _userJwtToken,
            UserRole.Admin => _adminJwtToken,
            _ => throw new ArgumentOutOfRangeException(nameof(userRole), "Invalid role specified.")
        });

        return _apiHttpClient.Client;
    }

    public IApiClient CreateUnauthenticatedClient() => _apiHttpClient.Client;

    private void InitilizeTokens()
    {
        _userJwtToken = JwtTokenMock.CreateToken(UserFactory.Sub, UserRole.User);
        _adminJwtToken = JwtTokenMock.CreateToken(UserFactory.Sub, UserRole.Admin);
    }
}
