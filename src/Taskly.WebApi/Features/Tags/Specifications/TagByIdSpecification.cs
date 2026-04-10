using Ardalis.Specification;
using TagId = Taskly.WebApi.Features.Tags.Models.TagId;
using UserId = Taskly.WebApi.Features.Users.Models.UserId;

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