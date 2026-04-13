using System.Diagnostics.CodeAnalysis;
using System.Windows.Threading;
using Auth0.OidcClient;
using Microsoft.Extensions.Configuration;
using Taskly.Desktop.Common.Composition.Extensions;
using Taskly.Desktop.Common.Composition.Options;
using Taskly.Desktop.Common.Infrastructure.Authentication;
using Taskly.Shared.Extensions;
using Taskly.WebApi.Client.Abstractions;
using Taskly.WebApi.Client.Common.Extensions;
using Wpf.Ui.DependencyInjection;

namespace Taskly.Desktop;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
[SuppressMessage("ReSharper", "AsyncVoidEventHandlerMethod")]
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

            services.AddSingleton<IThemeService, ThemeService>();
            services.AddSingleton<ITaskBarService, TaskBarService>();
            services.AddSingleton<INavigationService, NavigationService>();

            services.AddPages(typeof(App).Assembly);

            services.AddApiHttpClient(options =>
            {
                var apiConfig = context.Configuration.GetOptions<ApiConfig>();

                options.BaseAddress = new Uri(apiConfig.BaseAddress);
                options.Timeout = TimeSpan.FromSeconds(apiConfig.TimeoutInSeconds);
            });

            services.AddSingleton(sp =>
            {
                var auth0Config = context.Configuration.GetOptions<Auth0Config>();

                return new AuthenticationService(new Auth0ClientOptions
                    {
                        Domain = auth0Config.Domain,
                        ClientId = auth0Config.ClientId
                    },
                    sp.GetRequiredService<IApiHttpClient>());
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
        var authService = Services.GetRequiredService<AuthenticationService>();
        await authService.LoginAsync();

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