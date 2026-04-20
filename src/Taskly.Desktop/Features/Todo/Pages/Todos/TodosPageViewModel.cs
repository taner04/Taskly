using Taskly.Desktop.Features.Main;
using Taskly.Desktop.Features.Todo.Pages.EditTodo;

namespace Taskly.Desktop.Features.Todo.Pages.Todos;

public sealed class TodosPageViewModel(
    EditTodoPageViewModel editTodoPageViewModel,
    INavigationService navigationService,
    MainWindowViewModel mainWindowViewModel)
{
}