using Taskly.WebApi.Features.Tags.Models;

namespace Taskly.WebApi.Common.Shared.Dtos;

public sealed record TagDto(
    Guid Id,
    string Name,
    Guid UserId
)
{
    internal static TagDto FromDomain(
        Tag tag) =>
        new(
            tag.Id.Value,
            tag.Name,
            tag.UserId.Value
        );
}