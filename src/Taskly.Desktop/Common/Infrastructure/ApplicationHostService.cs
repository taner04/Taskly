using Taskly.Desktop.Common.Shared;
using Taskly.Desktop.Features.Home;
using Taskly.Desktop.Features.Main;
using Taskly.WebApi.Client.Common;

namespace Taskly.Desktop.Common.Infrastructure;

/// <summary>
///     Managed host of the application.
/// </summary>
public class ApplicationHostService(IServiceProvider serviceProvider, AuthenticationService authenticationService, WebClientService webClientService)
    : IHostedService
{
    /// <summary>
    ///     Triggered when the application host is ready to start the service.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        await HandleActivationAsync();
    }

    /// <summary>
    ///     Triggered when the application host is performing a graceful shutdown.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await authenticationService.LogoutAsync();
    }

    /// <summary>
    ///     Creates main window during activation.
    /// </summary>
    private async Task HandleActivationAsync()
    {
        if (Application.Current.Windows.OfType<MainWindow>().Any())
        {
            return;
        }

        var navigationWindow = serviceProvider.GetRequiredService<INavigationWindow>();

        _ = await authenticationService.LogoutAsync(); // Clear cookies on first run
        
        bool authenticated;
        do
        {
            authenticated = await authenticationService.LoginAsync();

            if (authenticated)
            {
                continue;
            }

            var retry = await TasklyMessageBox.ShowYesOrNoAsync(
                "Authentication failed. Do you want to retry?",
                "Authentication Failed");

            if (retry)
            {
                continue;
            }

            Application.Current.Shutdown();
            return;
        } while (!authenticated);

        navigationWindow.ShowWindow();
        navigationWindow.Navigate(typeof(HomePage));

        await Task.CompletedTask;
    }
}