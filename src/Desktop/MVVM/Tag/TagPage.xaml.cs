using Desktop.Attributes;
using Wpf.Ui.Abstractions.Controls;

namespace Desktop.MVVM.Tag;

/// <summary>
///     Interaction logic for TagPage.xaml
/// </summary>
[RequiresAuthorizedUser]
[PageRegistration(typeof(TagPageViewModel))]
public partial class TagPage : INavigableView<TagPageViewModel>
{
    public TagPage(
        TagPageViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }

    public TagPageViewModel ViewModel { get; }
}