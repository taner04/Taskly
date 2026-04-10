namespace Taskly.Desktop.MVVM.Home;

[PageInjection(typeof(HomePageViewModel))]
public partial class HomePage : INavigableView<HomePageViewModel>
{
    public HomePage(HomePageViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }

    public HomePageViewModel ViewModel { get; }
}