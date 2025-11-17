using FluentValidation;

namespace Taskly.Api.Features.TodoAggregate.UpdateTodo;

public sealed class UpdateTodoCommandValidator : AbstractValidator<UpdateTodoCommand>
{
    public UpdateTodoCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.DueDate).NotEmpty();
        RuleFor(x => x.Priority).NotEmpty();
        RuleFor(x => x.IsCompleted).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
    }
}