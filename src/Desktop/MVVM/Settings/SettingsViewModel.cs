using Desktop.MVVM.Main;
using Desktop.MVVM.Settings.Models;
using Desktop.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Wpf.Ui;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace Desktop.MVVM.Settings;

public partial class SettingsPageViewModel(
    INavigationService navigationService, 
    Auth0Service auth0Service) : PageViewModelBase(navigationService, auth0Service)
{
    [ObservableProperty] private string _appVersion = string.Empty;

    [ObservableProperty] private List<WindowBackdropType> _backdropTypes = [];

    [ObservableProperty] private List<string> _exportTypes = [];

    private WindowBackdropType _selectedBackdropType;

    [ObservableProperty] private string _selectedExportType = null!;

    private ApplicationTheme _selectedTheme;

    [ObservableProperty] private List<ApplicationTheme> _themes = [];

    public WindowBackdropType SelectedBackdropType
    {
        get => _selectedBackdropType;
        set
        {
            if (SetProperty(ref _selectedBackdropType, value))
            {
                AppConfig.AppTheme.BackdropType = value;
                var mainWindow = App.Services.GetRequiredService<INavigationWindow>() as MainWindow;
                WindowBackdrop.ApplyBackdrop(mainWindow, value);
            }
        }
    }

    public ApplicationTheme SelectedTheme
    {
        get => _selectedTheme;
        set
        {
            if (SetProperty(ref _selectedTheme, value))
            {
                AppConfig.AppTheme.Theme = value;
                ApplicationThemeManager.Apply(value);
            }
        }
    }

    public override string Title => throw new NotImplementedException();

    protected override async Task InitializeAsync()
    {
        AppVersion = $"Taskly - {GetAssemblyVersion()}";

        BackdropTypes = [.. Enum.GetValues<WindowBackdropType>()];
        Themes = [.. Enum.GetValues<ApplicationTheme>()];

        Themes.Remove(ApplicationTheme.Unknown);
        Themes.Remove(ApplicationTheme.HighContrast);

        SelectedBackdropType = AppConfig.AppTheme.BackdropType;
        SelectedTheme = AppConfig.AppTheme.Theme;

        await Task.CompletedTask;
    }

    private static string GetAssemblyVersion()
    {
        return Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "";
    }
}
