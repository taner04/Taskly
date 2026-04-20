namespace Taskly.Desktop.Features.Todo.Pages.Todos;

public partial class TodosPage : INavigableView<TodosPageViewModel>
{
    public TodosPage(TodosPageViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }

    public TodosPageViewModel ViewModel { get; }
}