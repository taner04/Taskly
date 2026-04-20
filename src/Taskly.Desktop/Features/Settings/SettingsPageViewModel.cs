using System.Reflection;
using Taskly.Desktop.Features.Main;
using Taskly.Desktop.Features.Settings.Models;

namespace Taskly.Desktop.Features.Settings;

public partial class SettingsPageViewModel : ObservableObject
{
    [ObservableProperty] public partial string AppVersion { get; set; }
    [ObservableProperty] public partial List<WindowBackdropType> BackdropTypes { get; set; }
    [ObservableProperty] public partial List<string> ExportTypes { get; set; } = [];
    [ObservableProperty] public partial string SelectedExportType { get; set; } = null!;
    [ObservableProperty] public partial List<ApplicationTheme> Themes { get; set; }

    public SettingsPageViewModel()
    {
        AppVersion = $"Taskly - {Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? ""}";

        BackdropTypes = [.. Enum.GetValues<WindowBackdropType>()];
        Themes = [.. Enum.GetValues<ApplicationTheme>()];

        Themes.Remove(ApplicationTheme.Unknown);
        Themes.Remove(ApplicationTheme.HighContrast);

        SelectedBackdropType = AppConfig.Instance.AppTheme.BackdropType;
        SelectedTheme = AppConfig.Instance.AppTheme.Theme;
    }

    public WindowBackdropType SelectedBackdropType
    {
        get;
        set
        {
            if (SetProperty(ref field, value))
            {
                AppConfig.Instance.AppTheme.BackdropType = value;
                var mainWindow = App.Services.GetRequiredService<INavigationWindow>() as MainWindow;
                WindowBackdrop.ApplyBackdrop(mainWindow, value);
            }
        }
    }

    public ApplicationTheme SelectedTheme
    {
        get;
        set
        {
            if (SetProperty(ref field, value))
            {
                AppConfig.Instance.AppTheme.Theme = value;
                ApplicationThemeManager.Apply(value);
            }
        }
    }
}