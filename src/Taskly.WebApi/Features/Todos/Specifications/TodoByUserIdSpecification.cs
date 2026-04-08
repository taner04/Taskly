using Taskly.WebApi.Features.Users.Model;
using Ardalis.Specification;

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
