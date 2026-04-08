using Taskly.Desktop.Common.Attributes;
using Wpf.Ui.Abstractions.Controls;

namespace Taskly.Desktop.MVVM.Tag;

[PageInjection(typeof(TagPageViewModel))]
public partial class TagPage : INavigableView<TagPageViewModel>
{
    public TagPage(TagPageViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }

    public TagPageViewModel ViewModel { get; }
}
