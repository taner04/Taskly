using Ardalis.Specification;
using TodoId = Taskly.WebApi.Features.Todos.Models.TodoId;
using UserId = Taskly.WebApi.Features.Users.Models.UserId;

namespace Taskly.WebApi.Features.Todos.Specifications;

internal sealed class TodoByUserIdWithTagsSpecification : TodoByUserIdSpecification
{
    public TodoByUserIdWithTagsSpecification(
        TodoId todoId,
        UserId userId) : base(todoId, userId)
    {
        Query.Include(t => t.Tags);
    }
}