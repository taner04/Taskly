using Taskly.Desktop.Common.Infrastructure.Auth0;
using Taskly.Desktop.Common.Infrastructure.Auth0.EventArgs;
using Taskly.Desktop.MVVM.Todo;
using Taskly.WebApi.Client.Abstractions;
using Wpf.Ui;

namespace Taskly.Desktop.MVVM.Home;

public partial class HomePageViewModel : ObservableObject, IDisposable
{
    private readonly IApiHttpClient _apiHttpClient;
    private readonly Auth0Service _auth0Service;
    private readonly INavigationService _navigationService;

    public HomePageViewModel(
        IApiHttpClient apiClient,
        Auth0Service auth0Service,
        INavigationService navigationService)
    {
        _apiHttpClient = apiClient;
        _navigationService = navigationService;
        _auth0Service = auth0Service;

        _auth0Service.OnSignIn += Auth0Service_OnSignIn;
        _auth0Service.OnSignOut += Auth0Service_OnSignOut;
        _auth0Service.OnError += Auth0Service_OnError;
    }


    public void Dispose()
    {
        _auth0Service.OnSignIn -= Auth0Service_OnSignIn;
        _auth0Service.OnSignOut -= Auth0Service_OnSignOut;
        _auth0Service.OnError -= Auth0Service_OnError;

        GC.SuppressFinalize(this);
    }

    private void Auth0Service_OnError(object? sender, SignInErrorEventArgs e)
    {
        //Display error message to the user
    }

    private void Auth0Service_OnSignOut(object? sender, SignOutEventArgs e)
    {
        //Clear access token and user information from memory
        Dispose();
        _apiHttpClient.ClearAccessToken();
        //Navigate to HomePage
        _navigationService.Navigate(typeof(HomePage));
    }

    private void Auth0Service_OnSignIn(object? sender, SignInEventArgs e)
    {
        // Store access token
        _apiHttpClient.SetAccessToken(e.AccessToken);
        //Navigate to Todo-Page
        _navigationService.Navigate(typeof(TodoPage));
    }

    [RelayCommand]
    private async Task SignIn(CancellationToken cancellationToken) => await _auth0Service.LoginAsync(cancellationToken);
}
