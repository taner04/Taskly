using Api.Features.Tags.Exceptions;
using Api.Features.Tags.Model;

namespace UnitTests.Tests;

public sealed class TagTests
{
    private const string ValidName = "ValidName";
    private const string ValidUserId = "user123";

    private const string TooShortName = "ab"; // length = 2 < MinNameLength
    private static readonly string TooLongName = new('a', Tag.MaxNameLength + 1);

    [Fact]
    public void Constructor_WithValidData_ShouldCreateTag()
    {
        var tag = new Tag(ValidName, ValidUserId);

        Assert.Equal(ValidName, tag.Name);
        Assert.Equal(ValidUserId, tag.UserId);
        Assert.NotEqual(Guid.Empty, tag.Id.Value);
    }

    [Fact]
    public void Constructor_WithNameTooShort_ShouldThrow()
    {
        Assert.Throws<TagInvalidNameException>(() => new Tag(TooShortName, ValidUserId));
    }

    [Fact]
    public void Constructor_WithNameTooLong_ShouldThrow()
    {
        Assert.Throws<TagInvalidNameException>(() => new Tag(TooLongName, ValidUserId));
    }

    [Fact]
    public void Constructor_WithNameAtMinLength_ShouldCreateTag()
    {
        var name = new string('a', Tag.MinNameLength);

        var tag = new Tag(name, ValidUserId);

        Assert.Equal(name, tag.Name);
    }

    [Fact]
    public void Constructor_WithNameAtMaxLength_ShouldCreateTag()
    {
        var name = new string('a', Tag.MaxNameLength);

        var tag = new Tag(name, ValidUserId);

        Assert.Equal(name, tag.Name);
    }

    [Fact]
    public void Rename_WithValidName_ShouldUpdateName()
    {
        var tag = new Tag(ValidName, ValidUserId);
        var updated = "UpdatedName";

        tag.Rename(updated);

        Assert.Equal(updated, tag.Name);
    }

    [Fact]
    public void Rename_WithNameTooShort_ShouldThrow()
    {
        var tag = new Tag(ValidName, ValidUserId);

        Assert.Throws<TagInvalidNameException>(() => tag.Rename(TooShortName));
    }

    [Fact]
    public void Rename_WithNameTooLong_ShouldThrow()
    {
        var tag = new Tag(ValidName, ValidUserId);

        Assert.Throws<TagInvalidNameException>(() => tag.Rename(TooLongName));
    }

    [Fact]
    public void Todos_Default_ShouldBeEmpty()
    {
        var tag = new Tag(ValidName, ValidUserId);

        Assert.Empty(tag.Todos);
    }
}