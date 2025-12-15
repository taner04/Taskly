using System.Configuration;
using Auth0.OidcClient;
using Desktop.Attributes;
using Desktop.MVVM.Home;
using Wpf.Ui;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;

namespace Desktop.Services.Auth0;

public sealed class Auth0Service(
    ISnackbarService snackbarService,
    INavigationService navigationService)
{
    private readonly Auth0Client _client = new(new Auth0ClientOptions
    {
        Domain = ConfigurationManager.AppSettings["Auth0:Domain"] ??
                 throw new InvalidOperationException("Auth0:Domain missing"),
        ClientId = ConfigurationManager.AppSettings["Auth0:ClientId"] ??
                   throw new InvalidOperationException("Auth0:ClientId missing"),
        Scope = "openid profile email roles",
        LoadProfile = true
    });

    public UserContext? CurrentUser { get; private set; }

    public bool IsAuthenticated => CurrentUser is not null;

    public event Action<UserContext?> AuthenticationStateChanged = null!;

    public async Task<bool> LoginAsync()
    {
        var result = await _client.LoginAsync();

        if (result.IsError)
        {
            snackbarService.Show("Login failed", "Please try again.", ControlAppearance.Danger);
            return false;
        }

        CurrentUser = UserContext.FromLoginResult(result);

        snackbarService.Show(
            "Login successful",
            "You're now logged in",
            ControlAppearance.Success
        );

        AuthenticationStateChanged?.Invoke(CurrentUser);
        return true;
    }

    public async Task LogoutAsync()
    {
        await _client.LogoutAsync();
        snackbarService.Show(
            "Loginout successful",
            "You're now logged out",
            ControlAppearance.Success
        );

        navigationService.Navigate(typeof(HomePage));
        CurrentUser = null;
        AuthenticationStateChanged?.Invoke(CurrentUser);
    }


    public void OnNavigatingEventHandler(
        NavigationView sender,
        NavigatingCancelEventArgs args)
    {
        var requiresAuth = Attribute.IsDefined(
            args.Page.GetType(),
            typeof(RequiresAuthorizedUserAttribute)
        );

        if (requiresAuth && !IsAuthenticated)
        {
            snackbarService.Show(
                "Unauthorized access",
                "You must be logged in to access this page.",
                ControlAppearance.Danger,
                null,
                TimeSpan.FromSeconds(5)
            );

            args.Cancel = true;
            sender.Navigate(typeof(HomePage));
        }
    }
}