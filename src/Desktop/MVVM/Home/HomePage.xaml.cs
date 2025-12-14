using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf.Ui.Abstractions.Controls;

namespace Desktop.MVVM.Home;
/// <summary>
/// Interaction logic for HomePage.xaml
/// </summary>
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
