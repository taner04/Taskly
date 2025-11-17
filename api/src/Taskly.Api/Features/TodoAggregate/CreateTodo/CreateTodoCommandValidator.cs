using FluentValidation;

namespace Taskly.Api.Features.TodoAggregate.CreateTodo;

public sealed class CreateTodoCommandValidator : AbstractValidator<CreateTodoCommand>
{
    public CreateTodoCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.DueDate).NotEmpty();
        RuleFor(x => x.Priority).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
    }
}