using System.Collections.ObjectModel;
using Taskly.Desktop.Features.Home;
using Taskly.Desktop.Features.Settings;
using Taskly.Desktop.Features.Tag.Pages.Tags;
using Taskly.Desktop.Features.Todo.Pages.Todos;

namespace Taskly.Desktop.Features.Main;

public partial class MainWindowViewModel : ObservableObject
{
    public MainWindowViewModel()
    {
        IsProgressRingActive = false;
        ProgressRingVisibility = Visibility.Collapsed;

        FooterMenuItems = [CreateNavigationViewItem<SettingsPage>(SymbolRegular.Settings24)];

        MenuItems =
        [
            CreateNavigationViewItem<HomePage>(SymbolRegular.Home24),
            CreateNavigationViewItem<TodosPage>(SymbolRegular.Book24),
            CreateNavigationViewItem<TagsPage>(SymbolRegular.Tag24)
        ];

        TrayMenuItems = [new MenuItem { Header = "Home", Tag = "tray_home" }];
    }

    public string ApplicationTitle => "Taskly - Desktop";

    [ObservableProperty] public partial ObservableCollection<NavigationViewItem> FooterMenuItems { get; private set; }

    [ObservableProperty] public partial ObservableCollection<NavigationViewItem> MenuItems { get; private set; }

    [ObservableProperty] public partial ObservableCollection<MenuItem> TrayMenuItems { get; private set; }

    [ObservableProperty] public partial bool IsProgressRingActive { get; set; }

    [ObservableProperty] public partial Visibility ProgressRingVisibility { get; set; }

    private static NavigationViewItem CreateNavigationViewItem<T>(SymbolRegular symbolRegular) where T : class =>
        new()
        {
            Content = typeof(T).Name.Replace("Page", string.Empty),
            Icon = new SymbolIcon { Symbol = symbolRegular },
            TargetPageType = typeof(T)
        };

    public void SetProgressRingActive()
    {
        IsProgressRingActive = true;
        ProgressRingVisibility = Visibility.Visible;
    }

    public void SetProgressRingInactive()
    {
        IsProgressRingActive = false;
        ProgressRingVisibility = Visibility.Collapsed;
    }
}