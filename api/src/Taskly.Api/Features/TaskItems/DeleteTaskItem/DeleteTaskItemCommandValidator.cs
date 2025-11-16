using FluentValidation;

namespace Taskly.Api.Features.TaskItems.DeleteTaskItem;

public sealed class DeleteTaskItemCommandValidator : AbstractValidator<DeleteTaskItemCommand>
{
    public DeleteTaskItemCommandValidator()
    {
        RuleFor(x => x.TaskItemId).NotNull();
        RuleFor(x => x.UserId).NotNull();
    }
}