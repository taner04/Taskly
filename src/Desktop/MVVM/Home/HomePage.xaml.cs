using Desktop.Attributes;
using Wpf.Ui.Abstractions.Controls;

namespace Desktop.MVVM.Home;

/// <summary>
///     Interaction logic for HomePage.xaml
/// </summary>
[PageRegistration(typeof(HomePageViewModel))]
public partial class HomePage : INavigableView<HomePageViewModel>
{
    public HomePage(
        HomePageViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }

    public HomePageViewModel ViewModel { get; }
}