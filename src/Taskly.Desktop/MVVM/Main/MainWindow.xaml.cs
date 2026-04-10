using Wpf.Ui.Abstractions;

namespace Taskly.Desktop.MVVM.Main;

[ServiceInjection<MainWindow, INavigationWindow>(ServiceLifetime.Singleton)]
public partial class MainWindow : INavigationWindow
{
    public MainWindow(
        MainWindowViewModel viewModel,
        INavigationViewPageProvider navigationViewPageProvider,
        INavigationService navigationService
    )
    {
        ViewModel = viewModel;
        DataContext = this;

        SystemThemeWatcher.Watch(this);

        InitializeComponent();
        SetPageService(navigationViewPageProvider);

        navigationService.SetNavigationControl(RootNavigation);
    }

    public MainWindowViewModel ViewModel { get; }

    INavigationView INavigationWindow.GetNavigation() => throw new NotImplementedException();

    public void SetServiceProvider(IServiceProvider serviceProvider)
    {
    }

    /// <summary>
    ///     Raises the closed event.
    /// </summary>
    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);

        // Make sure that closing this window will begin the process of closing the application.
        Application.Current.Shutdown();
    }

    #region INavigationWindow methods

    public INavigationView GetNavigation() => RootNavigation;

    public bool Navigate(Type pageType) => RootNavigation.Navigate(pageType);

    public void SetPageService(INavigationViewPageProvider navigationViewPageProvider) =>
        RootNavigation.SetPageProviderService(navigationViewPageProvider);

    public void ShowWindow() => Show();

    public void CloseWindow() => Close();

    #endregion INavigationWindow methods
}