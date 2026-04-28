using Taskly.WebApi.Common.Shared.Models;
using Taskly.WebApi.Features.Tags.Common.Exceptions;
using UserId = Taskly.WebApi.Features.Users.Common.Models.UserId;

namespace Taskly.WebApi.Features.Tags.Common.Models;

[ValueObject<Guid>]
public readonly partial struct TagId;

public class Tag : Entity<TagId>
{
    public const int MaxNameLength = 50;
    public const int MinNameLength = 3;

    private Tag(
        string name,
        UserId userId)
    {
        Id = TagId.From(Guid.CreateVersion7());
        Name = name;
        UserId = userId;
    }

    public string Name { get; set; }
    public UserId UserId { get; init; }

    public ICollection<Todo> Todos { get; init; } = [];

    public static Tag Create(string name, UserId userId)
    {
        InvalidTagNameException.ThrowIfInvalid(name);

        return new Tag(name, userId);
    }
}