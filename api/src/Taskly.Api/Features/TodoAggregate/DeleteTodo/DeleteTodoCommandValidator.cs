using FluentValidation;

namespace Taskly.Api.Features.TodoAggregate.DeleteTodo;

public sealed class DeleteTodoCommandValidator : AbstractValidator<DeleteTodoCommand>
{
    public DeleteTodoCommandValidator()
    {
        RuleFor(x => x.TodoId).NotNull();
        RuleFor(x => x.UserId).NotNull();
    }
}