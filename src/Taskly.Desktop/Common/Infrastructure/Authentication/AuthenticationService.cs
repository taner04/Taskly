using System.Diagnostics;
using Auth0.OidcClient;
using Duende.IdentityModel.OidcClient;
using Taskly.WebApi.Client.Abstractions;

namespace Taskly.Desktop.Common.Infrastructure.Authentication;

public sealed class AuthenticationService(
    Auth0ClientOptions options,
    IHostedService hostedService,
    IApiHttpClient apiHttpClient)
{
    private readonly Auth0Client _auth0Client = new(options);

    public async Task OnStartUpAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            LoginResult loginResult;

            while (true)
            {
                loginResult = await _auth0Client.LoginAsync(cancellationToken);

                if (!loginResult.IsError)
                {
                    apiHttpClient.SetAccessToken(loginResult.AccessToken);
                    break;
                }

                if (loginResult.Error.Equals("UserCancel", StringComparison.OrdinalIgnoreCase))
                {
                    if (await MessageBox.ShowYesOrNoAsync("Authentication Cancelled",
                            "You cancelled the login process. Would you like to try again?", cancellationToken))
                    {
                        continue;
                    }

                    Application.Current.Shutdown();
                    return;
                }

                await MessageBox.ShowErrorAsync(
                    "Authentication Failed",
                    "Failed to authenticate your account. Please check your credentials and network connection, then try again.\n\nDetails: " +
                    loginResult.ErrorDescription,
                    cancellationToken
                );

                await Task.Delay(500, cancellationToken);
            }

            await hostedService.StartAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            await MessageBox.ShowErrorAsync(ex.ToString(),
                "Application Startup Error: Unable to initialize authentication", cancellationToken);
            Application.Current.Shutdown();
        }
    }
}