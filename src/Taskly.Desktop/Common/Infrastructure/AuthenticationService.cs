using Auth0.OidcClient;
using Duende.IdentityModel.OidcClient.Browser;
using Microsoft.Extensions.Options;
using Taskly.Desktop.Common.Composition.Options;
using Taskly.WebApi.Client.Common;

namespace Taskly.Desktop.Common.Infrastructure;

public sealed class AuthenticationService
{
    private readonly Auth0Client _client;
    private readonly Dictionary<string, string> _extraParameters;
    private readonly WebClientService _webClientService;

    public AuthenticationService(
        IOptions<Auth0Config> auth0Options,
        WebClientService webClientService)
    {
        _webClientService = webClientService;

        var config = auth0Options.Value;

        _client = new Auth0Client(new Auth0ClientOptions
        {
            Domain = config.Domain,
            ClientId = config.ClientId,
            Scope = config.Scope
        });

        _extraParameters = new Dictionary<string, string>
        {
            { "connection", config.ConnectionName },
            { "audience", config.Audience }
        };
    }

    public async Task<bool> LoginAsync()
    {
        var loginResult = await _client.LoginAsync(
            _extraParameters);

        if (!loginResult.IsError)
        {
            _webClientService.SetAccessToken(loginResult.AccessToken);
        }

        return !loginResult.IsError;
    }

    public async Task<bool> LogoutAsync()
    {
        var result = await _client.LogoutAsync();

        if (result == BrowserResultType.Success)
        {
            _webClientService.ClearAccessToken();
        }

        return result == BrowserResultType.Success;
    }
}