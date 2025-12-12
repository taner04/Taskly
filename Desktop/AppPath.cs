namespace Desktop;

public static class AppPath
{
    public const string AppName = "Taskly";

    public static string AppDataFolder =>
        $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\{AppName}";

    public static string AppThemeFile => $"{AppDataFolder}\\appTheme.json";
}
