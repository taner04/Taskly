using System.Diagnostics.CodeAnalysis;
using Taskly.WebApi.Common.Shared.Extensions;
using Taskly.WebApi.Common.Shared.Models;
using Taskly.WebApi.Features.Todos.Models;
using Taskly.WebApi.Features.Users.Models;

namespace Taskly.WebApi.Features.Tags.Models;

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
        name.EnsureLengthInRange<Tag>(MinNameLength, MaxNameLength, nameof(Name));
    }

    public void Rename(
        string newName)
    {
        Validate(newName);

        Name = newName;
    }
}