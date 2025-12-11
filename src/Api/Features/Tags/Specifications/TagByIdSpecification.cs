using Api.Features.Users.Model;
using Ardalis.Specification;

namespace Api.Features.Tags.Specifications;

public class TagByIdSpecification : Specification<Tag>
{
    public TagByIdSpecification(
        TagId todoId,
        UserId userId)
    {
        Query.Where(t => t.Id == todoId && t.UserId == userId);
    }
}