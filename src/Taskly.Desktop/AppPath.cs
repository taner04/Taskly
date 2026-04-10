namespace Taskly.Desktop;

public static class AppPath
{
    private const string AppName = "Taskly";

    public static string AppDataFolder =>
        $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\{AppName}";

    public static string AppThemeFile => $"{AppDataFolder}\\appTheme.json";
}