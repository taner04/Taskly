using ErrorOr;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Taskly.Api.Infrastructure.Data;

namespace Taskly.Api.Features.TodoAggregate.UpdateTodo;

public class UpdateTodoCommandHandler(ApplicationDbContext context)
    : ICommandHandler<UpdateTodoCommand, ErrorOr<Success>>
{
    public async ValueTask<ErrorOr<Success>> Handle(UpdateTodoCommand command, CancellationToken cancellationToken)
    {
        var todo = await context.Todos.SingleOrDefaultAsync(
            t => t.Id == command.TodoId && t.UserId == command.UserId, cancellationToken);

        if (todo is null)
            return Error.NotFound("Todo.NotFound",
                $"The todo does not exist with the specified id '{command.TodoId}'.");

        var updateTodoResult =
            todo.Update(command.Title, command.Description, command.DueDate, command.Priority, command.IsCompleted);

        if (updateTodoResult.IsError) return updateTodoResult;

        context.Todos.Update(todo);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}