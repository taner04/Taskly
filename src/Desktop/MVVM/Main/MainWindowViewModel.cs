using System.Collections.ObjectModel;
using Desktop.MVVM.Home;
using Desktop.MVVM.Settings;
using Desktop.MVVM.Todo;
using Wpf.Ui.Controls;

namespace Desktop.MVVM.Main
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        public override string Title => "Taskly - Desktop";


        [ObservableProperty]
        private ObservableCollection<object> _menuItems =
        [
            new NavigationViewItem()
            {
                Content = "Home",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Home24 },
                TargetPageType = typeof(HomePage)
            },
            new NavigationViewItem()
            {
                Content = "Todos",
                Icon = new SymbolIcon { Symbol = SymbolRegular.TaskListLtr24 },
                TargetPageType = typeof(TodoPage)
            },
            new NavigationViewItem()
            {
                Content = "Tags",
                Icon = new SymbolIcon { Symbol = SymbolRegular.TagQuestionMark24 },
                TargetPageType = typeof(TodoPage)
            }
        ];

        [ObservableProperty]
        private ObservableCollection<object> _footerMenuItems =
        [
            new NavigationViewItem()
            {
                Content = "Settings",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Settings24 },
                TargetPageType = typeof(SettingsPage)
            }
        ];

        [ObservableProperty]
        private ObservableCollection<MenuItem> _trayMenuItems =
        [
            new MenuItem { Header = "Home", Tag = "tray_home" }
        ];
    }
}
