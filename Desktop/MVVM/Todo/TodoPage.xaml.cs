using Desktop.Attributes;
using Wpf.Ui.Abstractions.Controls;

namespace Desktop.MVVM.Todo;

/// <summary>
/// Interaktionslogik für TodoPage.xaml
/// </summary>
[RequireAuthentication]
[PageInjection(typeof(TodoPageViewModel))]
public partial class TodoPage : INavigableView<TodoPageViewModel>
{
    public TodoPageViewModel ViewModel { get; }
    public TodoPage(TodoPageViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }
}
