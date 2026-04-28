using Ardalis.Specification;
using UserId = Taskly.WebApi.Features.Users.Common.Models.UserId;

namespace Taskly.WebApi.Features.Todos.Common.Specifications;

internal sealed class TodoByUserIdSpecification : Specification<Todo>
{
    public TodoByUserIdSpecification(
        TodoId todoId,
        UserId userId)
    {
        Query.Where(t => t.Id == todoId && t.UserId == userId).Include(t => t.Tags).Include(t => t.Attachments);
    }
}