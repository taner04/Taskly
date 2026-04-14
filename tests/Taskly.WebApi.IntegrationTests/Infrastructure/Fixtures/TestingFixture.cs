using System.Diagnostics.CodeAnalysis;
using Taskly.WebApi.Client.Abstractions;
using Taskly.WebApi.Client.Common;
using Taskly.WebApi.Features.Attachments.WebHooks;
using Taskly.WebApi.IntegrationTests.Factories;
using Taskly.WebApi.IntegrationTests.Infrastructure.Composition.Mocks.Jwt;
using Taskly.WebApi.IntegrationTests.Infrastructure.TestContainers.Azure;
using Taskly.WebApi.IntegrationTests.Infrastructure.TestContainers.Postgres;

namespace Taskly.WebApi.IntegrationTests.Infrastructure.Fixtures;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public sealed class TestingFixture : IAsyncLifetime
{
    internal const string WebHookSecret = "my-test-secret-key";

    private readonly AzureContainerHub _azureContainerHub = new();

    private readonly PostgresContainerHub _postgresContainerHub = new();

    private string _adminJwtToken = null!;
    private ApiHttpClient _apiHttpClient = null!;
    private IServiceScopeFactory _serviceScopeFactory = null!;
    private string _userJwtToken = null!;
    private WebApiFactory _webApiFactory = null!;
    private ApiHttpClient _webHookClient = null!;

    public async ValueTask InitializeAsync()
    {
        InitilizeTokens();
        await InitilizeContainersAsync();

        _webApiFactory = new WebApiFactory(_postgresContainerHub.DbConnection, _azureContainerHub.ConnectionString);
        _serviceScopeFactory = _webApiFactory.Services.GetRequiredService<IServiceScopeFactory>();

        _apiHttpClient = new ApiHttpClient(_webApiFactory.CreateClient());

        var client = _webApiFactory.CreateClient();
        client.DefaultRequestHeaders.Add(AttachmentWebHookConstants.RequestHeader, WebHookSecret);
        _webHookClient = new ApiHttpClient(client);
    }

    public async ValueTask DisposeAsync()
    {
        await _webApiFactory.DisposeAsync();
        await _postgresContainerHub.DisposeAsync();
        await _azureContainerHub.DisposeAsync();
    }

    public async Task SetUpAsync()
    {
        _apiHttpClient.ClearAccessToken();

        await _postgresContainerHub.ResetContainerAsync();
        await _azureContainerHub.ResetContainerAsync();
    }

    public async Task<UserId> CreateForeignUserAsync() => await _postgresContainerHub.CreateForeignUserAsync();

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

    public IApiClient GetUnauthenticatedClient() => _apiHttpClient.Client;

    public IApiClient GetWebHookClient() => _webHookClient.Client;

    private void InitilizeTokens()
    {
        _userJwtToken = JwtTokenMock.CreateToken(UserFactory.Sub, UserRole.User);
        _adminJwtToken = JwtTokenMock.CreateToken(UserFactory.Sub, UserRole.Admin);
    }

    private async ValueTask InitilizeContainersAsync()
    {
        await _postgresContainerHub.InitializeContainerAsync();
        await _azureContainerHub.InitializeContainerAsync();
    }
}