using System.Security.Claims;
using Auth0.OidcClient;
using Desktop.Mapper;
using Desktop.Shared;

namespace Desktop.Services;

public sealed class Auth0Service(Auth0ClientOptions options)
{
    private readonly Auth0Client _client = new(options);

    public UserContext? CurrentUser { get; private set; }

    public string? AccessToken { get; private set; }

    public bool IsAuthenticated => CurrentUser is not null;

    public async Task<bool> LoginAsync()
    {
        var result = await _client.LoginAsync();

        if (result.IsError)
        {
            return false;
        }

        CurrentUser = UserMapper.FromClaims(result.User);
        AccessToken = result.AccessToken;

        return true;
    }

    public async Task LogoutAsync()
    {
        await _client.LogoutAsync();

        CurrentUser = null;
        AccessToken = null;
    }
}