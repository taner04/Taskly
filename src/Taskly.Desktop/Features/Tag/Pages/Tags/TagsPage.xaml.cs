namespace Taskly.Desktop.Features.Tag.Pages.Tags;

public partial class TagsPage : INavigableView<TagsPageViewModel>
{
    public TagsPage(TagsPageViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }

    public TagsPageViewModel ViewModel { get; }
}