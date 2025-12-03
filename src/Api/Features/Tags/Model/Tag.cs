using System.Diagnostics.CodeAnalysis;
using Api.Features.Todos.Model;
using Api.Shared.Features.Models;

namespace Api.Features.Tags.Model;

[ValueObject<Guid>]
public readonly partial struct TagId;

[SuppressMessage("ReSharper", "EntityFramework.ModelValidation.UnlimitedStringLength")]
public class Tag : Entity<TagId>
{
    public const int MaxNameLength = 50;
    public const int MinNameLength = 3;

    public const int MaxUserIdLength = 256;

    private Tag(
        string name,
        string userId)
    {
        Id = TagId.From(Guid.CreateVersion7());
        Name = name;
        UserId = userId;
    }

    public string Name { get; private set; }
    public string UserId { get; init; }

    public ICollection<Todo> Todos { get; init; } = [];

    public static ErrorOr<Tag> TryCreate(
        string name,
        string userId)
    {
        if (name.Length is > MaxNameLength or < MinNameLength)
        {
            return Error.Conflict("Tag.Name",
                $"The name can not be longer than {MaxNameLength} characters or less than {MinNameLength} characters.");
        }

        if (string.IsNullOrEmpty(userId) || userId.Length > MaxUserIdLength)
        {
            return Error.Conflict("Tag.UserId",
                $"The user ID can not be longer than {MaxUserIdLength} characters.");
        }

        return new Tag(name, userId);
    }

    public ErrorOr<Success> Rename(
        string newName)
    {
        if (newName.Length is > MaxNameLength or < MinNameLength)
        {
            return Error.Conflict("Tag.Name",
                $"The name can not be longer than {MaxNameLength} characters or less than {MinNameLength} characters.");
        }

        Name = newName;
        return Result.Success;
    }
}