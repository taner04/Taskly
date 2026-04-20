using System.Diagnostics.CodeAnalysis;
using System.Windows.Threading;
using Microsoft.Extensions.Configuration;
using Taskly.Desktop.Common.Composition.Options;
using Taskly.Desktop.Common.Infrastructure;
using Taskly.Desktop.Features.Home;
using Taskly.Desktop.Features.Main;
using Taskly.Desktop.Features.Settings;
using Taskly.Desktop.Features.Tag.Pages.EditTag;
using Taskly.Desktop.Features.Tag.Pages.Tags;
using Taskly.Desktop.Features.Todo.Pages.EditTodo;
using Taskly.Desktop.Features.Todo.Pages.Todos;
using Taskly.Shared.Extensions;
using Taskly.WebApi.Client;
using Wpf.Ui.DependencyInjection;

namespace Taskly.Desktop;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App
{
    // ReSharper disable once InconsistentNaming
    private static readonly IHost _host = Host
        .CreateDefaultBuilder()
        .ConfigureAppConfiguration(c => { c.SetBasePath(Path.GetDirectoryName(AppContext.BaseDirectory)!); })
        .ConfigureServices((context, services) =>
        {
            services.AddNavigationViewPageProvider();

            // App Host
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

            services.AddSingleton<HomePage>();
            services.AddSingleton<HomePageViewModel>();
            services.AddSingleton<TodosPage>();
            services.AddSingleton<TodosPageViewModel>();
            services.AddSingleton<TagsPage>();
            services.AddSingleton<TagsPageViewModel>();
            services.AddSingleton<SettingsPage>();
            services.AddSingleton<SettingsPageViewModel>();

            services.AddSingleton<EditTodoPage>();
            services.AddSingleton<EditTodoPageViewModel>();

            services.AddSingleton<EditTagPage>();
            services.AddSingleton<EditTagPageViewModel>();

            var configuration = context.Configuration;
            var isTestEnv = context.HostingEnvironment.IsEnvironment("Testing");
            services.AddConfig<Auth0Config>(configuration, isTestEnv);

            services.AddWebApiClient(options =>
            {
                var apiConfig = configuration.GetOptions<ApiConfig>();

                options.BaseAddress = new Uri(apiConfig.BaseAddress);
                options.Timeout = TimeSpan.FromSeconds(apiConfig.TimeoutInSeconds);
            });

            services.AddSingleton<AuthenticationService>();
        }).Build();

    /// <summary>
    ///     Gets services.
    /// </summary>
    public static IServiceProvider Services => _host.Services;

    /// <summary>
    ///     Occurs when the application is loading.
    /// </summary>
    // ReSharper disable once AsyncVoidEventHandlerMethod
    private async void OnStartup(object sender, StartupEventArgs e)
    {
        await _host.StartAsync();
    }


    /// <summary>
    ///     Occurs when the application is closing.
    /// </summary>
    // ReSharper disable once AsyncVoidEventHandlerMethod
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