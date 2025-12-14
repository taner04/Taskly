using Desktop.Attributes;
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

namespace Desktop.MVVM.Tag;
/// <summary>
/// Interaction logic for TagPage.xaml
/// </summary>
/// 
[RequiresAuthorizedUser]
[PageRegistration(typeof(TagPageViewModel))]
public partial class TagPage : INavigableView<TagPageViewModel>
{
    public TagPageViewModel ViewModel { get; }

    public TagPage(TagPageViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }
}
