using Taskly.Desktop.Common.Composition.Extensions;

namespace Taskly.Desktop.MVVM.Settings.Models;

public class AppConfig : ObservableObject
{
    private static readonly Lazy<AppConfig> _instance = new(() => new AppConfig());

    private AppConfig()
    {
        EnsureDirectories();
        EnsureConfigFiles();

        AppTheme = File.ReadAllText(AppPath.AppThemeFile).FromJson<AppTheme>();
    }

    public static AppConfig Instance => _instance.Value;

    public AppTheme AppTheme { get; set; }

    private static void EnsureDirectories()
    {
        if (Directory.Exists(AppPath.AppDataFolder))
        {
            return;
        }

        Directory.CreateDirectory(AppPath.AppDataFolder);
    }

    private static void EnsureConfigFiles()
    {
        if (File.Exists(AppPath.AppThemeFile))
        {
            return;
        }

        AppTheme.CallbackTheme.ToJson(AppPath.AppThemeFile);
    }

    public void Save() => AppTheme.ToJson(AppPath.AppThemeFile);
}