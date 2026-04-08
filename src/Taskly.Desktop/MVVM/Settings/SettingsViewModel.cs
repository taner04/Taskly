using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Taskly.Desktop.MVVM.Main;
using Taskly.Desktop.MVVM.Settings.Models;
using Wpf.Ui;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace Taskly.Desktop.MVVM.Settings;

public partial class SettingsPageViewModel : ObservableObject
{
    [ObservableProperty] private string _appVersion = string.Empty;

    [ObservableProperty] private List<WindowBackdropType> _backdropTypes = [];

    [ObservableProperty] private List<string> _exportTypes = [];

    private WindowBackdropType _selectedBackdropType;

    [ObservableProperty] private string _selectedExportType = null!;

    private ApplicationTheme _selectedTheme;

    [ObservableProperty] private List<ApplicationTheme> _themes = [];

    public SettingsPageViewModel()
    {
        AppVersion = $"Taskly - {GetAssemblyVersion()}";

        BackdropTypes = [.. Enum.GetValues<WindowBackdropType>()];
        Themes = [.. Enum.GetValues<ApplicationTheme>()];

        Themes.Remove(ApplicationTheme.Unknown);
        Themes.Remove(ApplicationTheme.HighContrast);

        SelectedBackdropType = AppConfig.Instance.AppTheme.BackdropType;
        SelectedTheme = AppConfig.Instance.AppTheme.Theme;
    }

    public WindowBackdropType SelectedBackdropType
    {
        get => _selectedBackdropType;
        set
        {
            if (SetProperty(ref _selectedBackdropType, value))
            {
                AppConfig.Instance.AppTheme.BackdropType = value;
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
                AppConfig.Instance.AppTheme.Theme = value;
                ApplicationThemeManager.Apply(value);
            }
        }
    }

    private static string GetAssemblyVersion() => Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "";
}
