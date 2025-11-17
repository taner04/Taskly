using ErrorOr;
using Mediator;
using Taskly.Api.Infrastructure.Data;
using Taskly.Domain.TodoAggregate;

namespace Taskly.Api.Features.TodoAggregate.CreateTodo;

public class CreateTodoCommandHandler(ApplicationDbContext context)
    : ICommandHandler<CreateTodoCommand, ErrorOr<Success>>
{
    public async ValueTask<ErrorOr<Success>> Handle(CreateTodoCommand command, CancellationToken cancellationToken)
    {
        var createNewTodoResult = Todo.TryCreate(command.Title, command.Description, command.DueDate, command.Priority,
            command.UserId);

        if (createNewTodoResult.IsError) return createNewTodoResult.Errors;

        var newTodo = createNewTodoResult.Value;

        context.Todos.Add(newTodo!);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}