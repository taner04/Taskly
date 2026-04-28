using Ardalis.Specification;
using TagId = Taskly.WebApi.Features.Tags.Common.Models.TagId;
using UserId = Taskly.WebApi.Features.Users.Common.Models.UserId;

namespace Taskly.WebApi.Features.Tags.Common.Specifications;

internal sealed class TagByIdSpecification : Specification<Tag>
{
    public TagByIdSpecification(
        TagId tagId,
        UserId userId)
    {
        Query.Where(t => t.Id == tagId && t.UserId == userId);
    }
}