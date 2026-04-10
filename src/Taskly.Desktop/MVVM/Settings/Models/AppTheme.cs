namespace Taskly.Desktop.MVVM.Settings.Models;

public sealed class AppTheme(WindowBackdropType backdropType, ApplicationTheme theme)
{
    public WindowBackdropType BackdropType { get; set; } = backdropType;

    public ApplicationTheme Theme { get; set; } = theme;

    public static AppTheme CallbackTheme => new(WindowBackdropType.Mica, ApplicationTheme.Dark);
}