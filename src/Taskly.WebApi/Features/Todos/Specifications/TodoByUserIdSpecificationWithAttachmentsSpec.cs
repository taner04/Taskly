using Taskly.WebApi.Features.Users.Model;
using Ardalis.Specification;

namespace Taskly.WebApi.Features.Todos.Specifications;

internal sealed class TodoByUserIdSpecificationWithAttachmentsSpec : TodoByUserIdSpecification
{
    public TodoByUserIdSpecificationWithAttachmentsSpec(
        TodoId todoId,
        UserId userId) : base(todoId, userId)
    {
        Query.Include(t => t.Attachments);
    }
}
