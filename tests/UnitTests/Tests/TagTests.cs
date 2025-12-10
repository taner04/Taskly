using Api.Features.Tags.Exceptions;
using Api.Features.Tags.Model;
using Api.Features.Users.Model;

namespace UnitTests.Tests;

public sealed class TagTests
{
    private const string ValidName = "ValidName";

    private const string TooShortName = "ab"; // length = 2 < MinNameLength
    private static readonly string TooLongName = new('a', Tag.MaxNameLength + 1);
    private readonly UserId _validUserId = UserId.From(Guid.Parse("00000000-0000-0000-0000-000000000001"));

    [Fact]
    public void Constructor_WithValidData_ShouldCreateTag()
    {
        var tag = new Tag(ValidName, _validUserId);

        Assert.Equal(ValidName, tag.Name);
        Assert.Equal(_validUserId, tag.UserId);
        Assert.NotEqual(Guid.Empty, tag.Id.Value);
    }

    [Fact]
    public void Constructor_WithNameTooShort_ShouldThrow()
    {
        Assert.Throws<TagInvalidNameException>(() => new Tag(TooShortName, _validUserId));
    }

    [Fact]
    public void Constructor_WithNameTooLong_ShouldThrow()
    {
        Assert.Throws<TagInvalidNameException>(() => new Tag(TooLongName, _validUserId));
    }

    [Fact]
    public void Constructor_WithNameAtMinLength_ShouldCreateTag()
    {
        var name = new string('a', Tag.MinNameLength);

        var tag = new Tag(name, _validUserId);

        Assert.Equal(name, tag.Name);
    }

    [Fact]
    public void Constructor_WithNameAtMaxLength_ShouldCreateTag()
    {
        var name = new string('a', Tag.MaxNameLength);

        var tag = new Tag(name, _validUserId);

        Assert.Equal(name, tag.Name);
    }

    [Fact]
    public void Rename_WithValidName_ShouldUpdateName()
    {
        var tag = new Tag(ValidName, _validUserId);
        var updated = "UpdatedName";

        tag.Rename(updated);

        Assert.Equal(updated, tag.Name);
    }

    [Fact]
    public void Rename_WithNameTooShort_ShouldThrow()
    {
        var tag = new Tag(ValidName, _validUserId);

        Assert.Throws<TagInvalidNameException>(() => tag.Rename(TooShortName));
    }

    [Fact]
    public void Rename_WithNameTooLong_ShouldThrow()
    {
        var tag = new Tag(ValidName, _validUserId);

        Assert.Throws<TagInvalidNameException>(() => tag.Rename(TooLongName));
    }

    [Fact]
    public void Todos_Default_ShouldBeEmpty()
    {
        var tag = new Tag(ValidName, _validUserId);

        Assert.Empty(tag.Todos);
    }
}