using Mediator;
using Microsoft.EntityFrameworkCore;
using Taskly.Api.Infrastructure.Data;
using Taskly.Core.Functional;

namespace Taskly.Api.Features.TaskItems.DeleteTaskItem;

public class DeleteTaskItemCommandHandler(ApplicationDbContext context) : ICommandHandler<DeleteTaskItemCommand, Result>
{
    public async ValueTask<Result> Handle(DeleteTaskItemCommand command, CancellationToken cancellationToken)
    {
        var taskItem = await context.TaskItems.SingleOrDefaultAsync(
            t => t.Id == command.TaskItemId && t.UserId == command.UserId, cancellationToken);

        if (taskItem is null)
        {
            return Error.NotFound("TaskItem.NotFound", $"The task with the Id '{command.TaskItemId}' was not found.");
        }
        
        context.TaskItems.Remove(taskItem);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}