using System.Collections.ObjectModel;
using Taskly.Desktop.MVVM.Home;
using Taskly.Desktop.MVVM.Settings;

namespace Taskly.Desktop.MVVM.Main;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<object> _footerMenuItems =
    [
        new NavigationViewItem
        {
            Content = "Settings",
            Icon = new SymbolIcon { Symbol = SymbolRegular.Settings24 },
            TargetPageType = typeof(SettingsPage)
        }
    ];

    [ObservableProperty] private ObservableCollection<object> _menuItems =
    [
        new NavigationViewItem
        {
            Content = "Home",
            Icon = new SymbolIcon { Symbol = SymbolRegular.Home24 },
            TargetPageType = typeof(HomePage)
        }
    ];

    [ObservableProperty] private ObservableCollection<MenuItem> _trayMenuItems =
    [
        new() { Header = "Home", Tag = "tray_home" }
    ];

    private static NavigationViewItem CreateMenuItem<T>(string content, SymbolIcon icon) where T : class =>
        new()
        {
            Content = content,
            Icon = new SymbolIcon { Symbol = SymbolRegular.Settings24 },
            TargetPageType = typeof(SettingsPage)
        };
}