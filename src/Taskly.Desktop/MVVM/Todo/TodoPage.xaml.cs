using Taskly.Desktop.Common.Attributes;
using Wpf.Ui.Abstractions.Controls;

namespace Taskly.Desktop.MVVM.Todo;

[PageInjection(typeof(TodoPageViewModel))]
public partial class TodoPage : INavigableView<TodoPageViewModel>
{
    public TodoPage(TodoPageViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }

    public TodoPageViewModel ViewModel { get; }
}
