using ErrorOr;
using Mediator;
using Taskly.Api.Features.Shared;
using Taskly.Domain.TodoAggregate;

namespace Taskly.Api.Features.TodoAggregate.GetTodos;

public record GetTodosQuery : UserRequest, IQuery<ErrorOr<List<Todo>>>;