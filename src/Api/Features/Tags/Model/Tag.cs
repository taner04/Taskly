using System.Diagnostics.CodeAnalysis;
using Api.Features.Shared.Models;
using Api.Features.Tags.Exceptions;
using Api.Features.Users.Model;

namespace Api.Features.Tags.Model;

[ValueObject<Guid>]
public readonly partial struct TagId;

[SuppressMessage("ReSharper", "EntityFramework.ModelValidation.UnlimitedStringLength")]
public class Tag : Entity<TagId>
{
    public const int MaxNameLength = 50;
    public const int MinNameLength = 3;

    public Tag(
        string name,
        UserId userId)
    {
        Validate(name);

        Id = TagId.From(Guid.CreateVersion7());
        Name = name;
        UserId = userId;
    }

    public string Name { get; private set; }
    public UserId UserId { get; init; }

    public ICollection<Todo> Todos { get; init; } = [];

    private static void Validate(
        string name)
    {
        if (name.Length is > MaxNameLength or < MinNameLength)
        {
            throw new TagInvalidNameException(name.Length);
        }
    }

    public void Rename(
        string newName)
    {
        Validate(newName);

        Name = newName;
    }
}