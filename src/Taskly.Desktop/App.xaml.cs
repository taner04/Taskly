using System.IO;
using System.Windows.Threading;
using Auth0.OidcClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Taskly.Desktop.Common.Composition.Options;
using Taskly.Desktop.Common.Infrastructure;
using Taskly.Desktop.Common.Infrastructure.Auth0;
using Taskly.Desktop.MVVM.Main;
using Taskly.Shared.Extensions;
using Taskly.WebApi.Client.Common.Extensions;
using Wpf.Ui;
using Wpf.Ui.DependencyInjection;

namespace Taskly.Desktop;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App
{
    // The.NET Generic Host provides dependency injection, configuration, logging, and other services.
    // https://docs.microsoft.com/dotnet/core/extensions/generic-host
    // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
    // https://docs.microsoft.com/dotnet/core/extensions/configuration
    // https://docs.microsoft.com/dotnet/core/extensions/logging
    // ReSharper disable once InconsistentNaming
    private static readonly IHost _host = Host
        .CreateDefaultBuilder()
        .ConfigureAppConfiguration(c => { c.SetBasePath(Path.GetDirectoryName(AppContext.BaseDirectory)!); })
        .ConfigureServices((context, services) =>
        {
            services.AddNavigationViewPageProvider();

            services.AddHostedService<ApplicationHostService>();

            // Theme manipulation
            services.AddSingleton<IThemeService, ThemeService>();

            // TaskBar manipulation
            services.AddSingleton<ITaskBarService, TaskBarService>();

            // Service containing navigation, same as INavigationWindow... but without window
            services.AddSingleton<INavigationService, NavigationService>();

            // Main window with navigation
            services.AddSingleton<INavigationWindow, MainWindow>();
            services.AddSingleton<MainWindowViewModel>();

            services.AddSingleton<Auth0Service>(_ =>
            {
                var auth0Config = context.Configuration.GetOptions<Auth0Config>();

                return new Auth0Service(new Auth0ClientOptions
                {
                    Domain = auth0Config.Domain,
                    ClientId = auth0Config.ClientId
                });
            });

            services.AddApiHttpClient(options =>
            {
                var apiConfig = context.Configuration.GetOptions<ApiConfig>();
                
                options.BaseAddress = new Uri(apiConfig.BaseAddress);
                options.Timeout  = TimeSpan.FromSeconds(apiConfig.TimeoutInSeconds);
            });
        }).Build();

    /// <summary>
    ///     Gets services.
    /// </summary>
    public static IServiceProvider Services => _host.Services;

    /// <summary>
    ///     Occurs when the application is loading.
    /// </summary>
    private async void OnStartup(object sender, StartupEventArgs e)
    {
        await _host.StartAsync();
    }

    /// <summary>
    ///     Occurs when the application is closing.
    /// </summary>
    private async void OnExit(object sender, ExitEventArgs e)
    {
        await _host.StopAsync();

        _host.Dispose();
    }

    /// <summary>
    ///     Occurs when an exception is thrown by an application but not handled.
    /// </summary>
    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        // For more info see https://docs.microsoft.com/en-us/dotnet/api/system.windows.application.dispatcherunhandledexception?view=windowsdesktop-6.0
    }
}
