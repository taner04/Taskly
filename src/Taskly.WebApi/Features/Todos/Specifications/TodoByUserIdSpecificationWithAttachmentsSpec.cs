using Ardalis.Specification;
using UserId = Taskly.WebApi.Features.Users.Models.UserId;

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