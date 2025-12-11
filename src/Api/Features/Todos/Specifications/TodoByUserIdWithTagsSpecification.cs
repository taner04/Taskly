using Api.Features.Users.Model;
using Ardalis.Specification;

namespace Api.Features.Todos.Specifications;

public sealed class TodoByUserIdWithTagsSpecification : TodoByUserIdSpecification
{
    public TodoByUserIdWithTagsSpecification(
        TodoId todoId,
        UserId userId) : base(todoId, userId)
    {
        Query.Include(t => t.Tags);
    }
}