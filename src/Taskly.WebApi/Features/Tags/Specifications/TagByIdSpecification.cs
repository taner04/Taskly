using Taskly.WebApi.Features.Users.Model;
using Ardalis.Specification;

namespace Taskly.WebApi.Features.Tags.Specifications;

internal sealed class TagByIdSpecification : Specification<Tag>
{
    public TagByIdSpecification(
        TagId todoId,
        UserId userId)
    {
        Query.Where(t => t.Id == todoId && t.UserId == userId);
    }
}
