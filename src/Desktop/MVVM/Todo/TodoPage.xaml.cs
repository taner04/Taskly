using Desktop.Attributes;
using Desktop.MVVM.Tag;
using Wpf.Ui.Abstractions.Controls;

namespace Desktop.MVVM.Todo;
/// <summary>
/// Interaction logic for TodoPage.xaml
/// </summary>
/// 

[RequiresAuthorizedUser]
[PageRegistration(typeof(TodoPageViewModel))]
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
