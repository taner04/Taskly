using Ardalis.Specification;
using Taskly.WebApi.Features.Todos.Models;
using TodoId = Taskly.WebApi.Features.Todos.Models.TodoId;
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