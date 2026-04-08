using Auth0.OidcClient;
using Duende.IdentityModel.OidcClient.Browser;
using Taskly.Desktop.Common.Infrastructure.Auth0.EventArgs;

namespace Taskly.Desktop.Common.Infrastructure.Auth0;

public sealed class Auth0Service(Auth0ClientOptions options)
{
    private readonly Auth0Client _client = new(options);

    public event EventHandler<SignInEventArgs>? OnSignIn;
    public event EventHandler<SignOutEventArgs>? OnSignOut;
    public event EventHandler<SignInErrorEventArgs>? OnError;

    public async Task LoginAsync(CancellationToken cancellationToken)
    {
        var result = await _client.LoginAsync(cancellationToken: cancellationToken);

        if (result.IsError)
        {
            OnError?.Invoke(this, new SignInErrorEventArgs(result.Error));
        }
        else
        {
            OnSignIn?.Invoke(this, new SignInEventArgs(result.AccessToken));
        }
    }

    public async Task LogoutAsync(CancellationToken cancellationToken)
    {
        var result = await _client.LogoutAsync(cancellationToken: cancellationToken);

        if (result != BrowserResultType.Success)
        {
            OnError?.Invoke(this, new SignInErrorEventArgs(result switch
            {
                BrowserResultType.HttpError => "HTTP error during logout.",
                BrowserResultType.Timeout => "Logout process timed out.",
                BrowserResultType.UnknownError => "An unknown error occurred during logout.",
                _ => "An unexpected error occurred during logout."
            }));
        }
        else
        {
            OnSignOut?.Invoke(this, new SignOutEventArgs());
        }
    }
}
