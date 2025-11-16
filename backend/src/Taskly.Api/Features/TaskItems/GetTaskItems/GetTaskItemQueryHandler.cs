using Mediator;
using Microsoft.EntityFrameworkCore;
using Taskly.Api.Infrastructure.Data;
using Taskly.Domain.TaskItems;

namespace Taskly.Api.Features.TaskItems.GetTaskItems;

public class GetTaskItemQueryHandler(ApplicationDbContext context) : IQueryHandler<GetTaskItemQuery, List<TaskItem>>
{
    public async ValueTask<List<TaskItem>> Handle(GetTaskItemQuery query, CancellationToken cancellationToken)
    {
        return await context.TaskItems.Where(t => t.UserId ==  query.UserId).ToListAsync(cancellationToken);
    }
}