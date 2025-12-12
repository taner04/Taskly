using Desktop.Abstractions;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace Desktop.MVVM.Settings.Models;

public sealed class AppTheme(WindowBackdropType backdropType, ApplicationTheme theme) : IJsonExtension
{
    public WindowBackdropType BackdropType { get; set; } = backdropType;

    public ApplicationTheme Theme { get; set; } = theme;
}
