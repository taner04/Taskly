using ErrorOr;
using Mediator;
using Taskly.Api.Features.Shared;
using Taskly.Domain.TodoAggregate;

namespace Taskly.Api.Features.TodoAggregate.CreateTodo;

public record CreateTodoCommand(string Title, string Description, DateTime DueDate, TodoPriority Priority)
    : UserRequest, ICommand<ErrorOr<Success>>;