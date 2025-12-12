using Desktop.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace Desktop.MVVM.Settings.Models;

public class AppConfig : ObservableObject
{
    private static readonly Lazy<AppConfig> _instance = new(() => new AppConfig());

    private AppConfig()
    {
        if (!CheckDirectories())
        {
            AppTheme = new AppTheme(WindowBackdropType.Mica, ApplicationTheme.Dark);
        }
        else
        {
            AppTheme = File.ReadAllText(AppPath.AppThemeFile).FromJson<AppTheme>();
        }
    }

    public static AppConfig Instance => _instance.Value;

    public AppTheme AppTheme { get; set; }

    private static bool CheckDirectories()
    {
        if (!Directory.Exists(AppPath.AppDataFolder))
        {
            Directory.CreateDirectory(AppPath.AppDataFolder);
            return false;
        }

        return true;
    }

    public void Save()
        => AppTheme.ToJson(AppPath.AppThemeFile);
}