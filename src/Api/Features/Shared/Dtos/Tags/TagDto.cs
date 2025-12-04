namespace Api.Shared.Dtos.Tags;

public sealed record TagDto(
    Guid Id,
    string Name,
    string UserId
)
{
    public static TagDto FromDomain(
        Tag tag)
    {
        return new TagDto(
            tag.Id.Value,
            tag.Name,
            tag.UserId
        );
    }
}