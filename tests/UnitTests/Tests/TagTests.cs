using Api.Features.Tags.Domain;

namespace UnitTests.Tests;

public sealed class TagTests
{
    private const string ValidName = "Valid Tag";
    private const string UpdatedName = "Updated Tag";
    private const string ValidUserId = "user123";

    private const string InvalidShortName = "ab"; // too short (< MinNameLength)

    [Fact]
    public void TryCreate_WithValidData_ShouldReturnTag()
    {
        var result = Tag.TryCreate(ValidName, ValidUserId);

        Assert.False(result.IsError);
        var tag = result.Value;
        Assert.Equal(ValidName, tag.Name);
        Assert.Equal(ValidUserId, tag.UserId);
        Assert.Empty(tag.Todos);
    }

    [Fact]
    public void TryCreate_WithNameTooShort_ShouldReturnError()
    {
        var result = Tag.TryCreate(InvalidShortName, ValidUserId);

        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Code == "Tag.Name");
    }

    [Fact]
    public void TryCreate_WithNameTooLong_ShouldReturnError()
    {
        var longName = new string('a', Tag.MaxNameLength + 1);

        var result = Tag.TryCreate(longName, ValidUserId);

        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Code == "Tag.Name");
    }

    [Fact]
    public void TryCreate_WithNameAtMinLength_ShouldReturnSuccess()
    {
        var name = new string('a', Tag.MinNameLength);

        var result = Tag.TryCreate(name, ValidUserId);

        Assert.False(result.IsError);
        Assert.Equal(name, result.Value.Name);
    }

    [Fact]
    public void TryCreate_WithNameAtMaxLength_ShouldReturnSuccess()
    {
        var name = new string('a', Tag.MaxNameLength);

        var result = Tag.TryCreate(name, ValidUserId);

        Assert.False(result.IsError);
        Assert.Equal(name, result.Value.Name);
    }

    [Fact]
    public void TryCreate_WithEmptyUserId_ShouldReturnError()
    {
        var result = Tag.TryCreate(ValidName, "");

        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Code == "Tag.UserId");
    }

    [Fact]
    public void TryCreate_WithNullUserId_ShouldReturnError()
    {
        string? userId = null;

        var result = Tag.TryCreate(ValidName, userId!);

        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Code == "Tag.UserId");
    }

    [Fact]
    public void TryCreate_WithUserIdTooLong_ShouldReturnError()
    {
        var userId = new string('x', Tag.MaxUserIdLength + 1);

        var result = Tag.TryCreate(ValidName, userId);

        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Code == "Tag.UserId");
    }

    [Fact]
    public void TryCreate_WithUserIdAtMaxLength_ShouldReturnSuccess()
    {
        var userId = new string('x', Tag.MaxUserIdLength);

        var result = Tag.TryCreate(ValidName, userId);

        Assert.False(result.IsError);
        Assert.Equal(userId, result.Value.UserId);
    }

    [Fact]
    public void Rename_WithValidName_ShouldUpdate()
    {
        var tag = Tag.TryCreate(ValidName, ValidUserId).Value;

        var result = tag.Rename(UpdatedName);

        Assert.False(result.IsError);
        Assert.Equal(UpdatedName, tag.Name);
    }

    [Fact]
    public void Rename_WithNameTooShort_ShouldReturnError()
    {
        var tag = Tag.TryCreate(ValidName, ValidUserId).Value;

        var result = tag.Rename(InvalidShortName);

        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Code == "Tag.Name");
    }

    [Fact]
    public void Rename_WithNameTooLong_ShouldReturnError()
    {
        var tag = Tag.TryCreate(ValidName, ValidUserId).Value;
        var longName = new string('a', Tag.MaxNameLength + 1);

        var result = tag.Rename(longName);

        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Code == "Tag.Name");
    }

    [Fact]
    public void Rename_WithNameAtMinLength_ShouldUpdate()
    {
        var tag = Tag.TryCreate(ValidName, ValidUserId).Value;
        var name = new string('a', Tag.MinNameLength);

        var result = tag.Rename(name);

        Assert.False(result.IsError);
        Assert.Equal(name, tag.Name);
    }

    [Fact]
    public void Rename_WithNameAtMaxLength_ShouldUpdate()
    {
        var tag = Tag.TryCreate(ValidName, ValidUserId).Value;
        var name = new string('a', Tag.MaxNameLength);

        var result = tag.Rename(name);

        Assert.False(result.IsError);
        Assert.Equal(name, tag.Name);
    }

    [Fact]
    public void Todos_ShouldBeEmpty_OnInitialization()
    {
        var result = Tag.TryCreate(ValidName, ValidUserId);

        Assert.False(result.IsError);
        Assert.Empty(result.Value.Todos);
    }
}
