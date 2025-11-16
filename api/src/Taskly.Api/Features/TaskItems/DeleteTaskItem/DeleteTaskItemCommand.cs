using Mediator;
using Taskly.Api.Features.Shared;
using Taskly.Core.Functional;

namespace Taskly.Api.Features.TaskItems.DeleteTaskItem;

public record DeleteTaskItemCommand(Guid TaskItemId) : UserRequest, ICommand<Result>;