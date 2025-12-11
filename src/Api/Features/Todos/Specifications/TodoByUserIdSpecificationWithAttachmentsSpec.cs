using Api.Features.Users.Model;
using Ardalis.Specification;

namespace Api.Features.Todos.Specifications;

public sealed class TodoByUserIdSpecificationWithAttachmentsSpec : TodoByUserIdSpecification
{
    public TodoByUserIdSpecificationWithAttachmentsSpec(
        TodoId todoId,
        UserId userId) : base(todoId, userId)
    {
        Query.Include(t => t.Attachments);
    }
}