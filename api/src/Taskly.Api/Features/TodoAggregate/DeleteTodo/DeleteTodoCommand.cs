using ErrorOr;
using Mediator;
using Taskly.Api.Features.Shared;

namespace Taskly.Api.Features.TodoAggregate.DeleteTodo;

public record DeleteTodoCommand(Guid TodoId) : UserRequest, ICommand<ErrorOr<Success>>;