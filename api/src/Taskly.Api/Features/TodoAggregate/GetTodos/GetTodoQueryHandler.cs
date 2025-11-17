using ErrorOr;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Taskly.Api.Infrastructure.Data;
using Taskly.Domain.TodoAggregate;

namespace Taskly.Api.Features.TodoAggregate.GetTodos;

public sealed class GetTodoQueryHandler(ApplicationDbContext context)
    : IQueryHandler<GetTodosQuery, ErrorOr<List<Todo>>>
{
    public async ValueTask<ErrorOr<List<Todo>>> Handle(GetTodosQuery query, CancellationToken cancellationToken)
    {
        return await context.Todos.Where(t => t.UserId == query.UserId).ToListAsync(cancellationToken);
    }
}