using Ardalis.Specification;
using UserId = Taskly.WebApi.Features.Users.Models.UserId;

namespace Taskly.WebApi.Features.Todos.Specifications;

internal class TodoByUserIdSpecification : Specification<Todo>
{
    public TodoByUserIdSpecification(
        TodoId todoId,
        UserId userId)
    {
        Query.Where(t => t.Id == todoId && t.UserId == userId);
    }
}