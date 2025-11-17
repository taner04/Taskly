using ErrorOr;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Taskly.Api.Infrastructure.Data;

namespace Taskly.Api.Features.TodoAggregate.DeleteTodo;

public class DeleteTodoCommandHandler(ApplicationDbContext context)
    : ICommandHandler<DeleteTodoCommand, ErrorOr<Success>>
{
    public async ValueTask<ErrorOr<Success>> Handle(DeleteTodoCommand command, CancellationToken cancellationToken)
    {
        var taskItem = await context.Todos.SingleOrDefaultAsync(
            t => t.Id == command.TodoId && t.UserId == command.UserId, cancellationToken);

        if (taskItem is null)
            return Error.NotFound("TaskItem.NotFound", $"The task with the Id '{command.TodoId}' was not found.");

        context.Todos.Remove(taskItem);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}