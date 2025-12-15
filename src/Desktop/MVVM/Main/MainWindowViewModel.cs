using System.Collections.ObjectModel;
using Desktop.MVVM.Account;
using Desktop.MVVM.Home;
using Desktop.MVVM.Todo;
using Desktop.Services.Auth0;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace Desktop.MVVM.Main;

public partial class MainWindowViewModel(
    ISnackbarService snackbarService,
    Auth0Service auth0Service) : ViewModelBase(snackbarService, auth0Service)
{
    [ObservableProperty] private ObservableCollection<NavigationViewItem> _footerMenuItems =
    [
        new()
        {
            Content = "Account",
            Icon = new SymbolIcon { Symbol = SymbolRegular.BookContacts24 },
            TargetPageType = typeof(AccountPage)
        }
    ];


    [ObservableProperty] private ObservableCollection<NavigationViewItem> _menuItems =
    [
        new()
        {
            Content = "Home",
            Icon = new SymbolIcon { Symbol = SymbolRegular.Home24 },
            TargetPageType = typeof(HomePage)
        },
        new()
        {
            Content = "Todos",
            Icon = new SymbolIcon { Symbol = SymbolRegular.TaskListLtr24 },
            TargetPageType = typeof(TodoPage)
        },
        new()
        {
            Content = "Tags",
            Icon = new SymbolIcon { Symbol = SymbolRegular.TagQuestionMark24 },
            TargetPageType = typeof(TodoPage)
        }
    ];

    [ObservableProperty] private ObservableCollection<MenuItem> _trayMenuItems =
    [
        new() { Header = "Home", Tag = "tray_home" }
    ];

    public override string Title => "Taskly - Desktop";
}