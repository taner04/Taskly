using Mediator;
using Taskly.Api.Features.Shared;
using Taskly.Domain.TaskItems;

namespace Taskly.Api.Features.TaskItems.GetTaskItems;

public record GetTaskItemQuery : UserRequest, IQuery<List<TaskItem>>;