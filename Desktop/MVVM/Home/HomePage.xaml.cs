using Desktop.Attributes;
using Wpf.Ui.Abstractions.Controls;

namespace Desktop.MVVM.Home;

/// <summary>
/// Interaktionslogik für HomePage.xaml
/// </summary>
[PageInjection(typeof(HomePageViewModel))]
public partial class HomePage : INavigableView<HomePageViewModel>
{
    public HomePageViewModel ViewModel { get; }

    public HomePage(HomePageViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }
}
