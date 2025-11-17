using ErrorOr;
using Mediator;
using Taskly.Api.Features.Shared;
using Taskly.Domain.TodoAggregate;

namespace Taskly.Api.Features.TodoAggregate.UpdateTodo;

public record UpdateTodoCommand(
    Guid TodoId,
    string Title,
    string Description,
    DateTime DueDate,
    TodoPriority Priority,
    bool IsCompleted) : UserRequest, ICommand<ErrorOr<Success>>;