using Api.Features.Todos.Exceptions;
using Api.Features.Todos.Model;
using Api.Features.Users.Model;

namespace UnitTests.Tests;

public sealed class TodoTests
{
    private const string ValidTitle = "Valid Todo";
    private const string ValidDescription = "Valid description";
    private readonly UserId _validUserId = UserId.From(Guid.Parse("00000000-0000-0000-0000-000000000001"));

    private const string TooShortString = "ab"; // length = 2 < Min
    private static readonly string TooLongTitle = new('x', Todo.MaxTitleLength + 1);
    private static readonly string TooLongDescription = new('x', Todo.MaxDescriptionLength + 1);

    [Fact]
    public void Constructor_WithValidData_ShouldCreateTodo()
    {
        var todo = new Todo(ValidTitle, ValidDescription, TodoPriority.Medium, _validUserId);

        Assert.Equal(ValidTitle, todo.Title);
        Assert.Equal(ValidDescription, todo.Description);
        Assert.Equal(TodoPriority.Medium, todo.Priority);
        Assert.False(todo.IsCompleted);
        Assert.Equal(_validUserId, todo.UserId);

        Assert.NotEqual(Guid.Empty, todo.Id.Value);
    }

    [Fact]
    public void Constructor_WithTooShortTitle_ShouldThrow()
    {
        Assert.Throws<TodoInvalidTitleException>(() =>
            new Todo(TooShortString, ValidDescription, TodoPriority.Low, _validUserId));
    }

    [Fact]
    public void Constructor_WithTooLongTitle_ShouldThrow()
    {
        Assert.Throws<TodoInvalidTitleException>(() =>
            new Todo(TooLongTitle, ValidDescription, TodoPriority.Low, _validUserId));
    }

    [Fact]
    public void Constructor_WithTitleAtMinLength_ShouldCreate()
    {
        var title = new string('a', Todo.MinTitleLength);

        var todo = new Todo(title, ValidDescription, TodoPriority.Low, _validUserId);

        Assert.Equal(title, todo.Title);
    }

    [Fact]
    public void Constructor_WithTitleAtMaxLength_ShouldCreate()
    {
        var title = new string('a', Todo.MaxTitleLength);

        var todo = new Todo(title, ValidDescription, TodoPriority.Low, _validUserId);

        Assert.Equal(title, todo.Title);
    }

    [Fact]
    public void Constructor_WithTooShortDescription_ShouldThrow()
    {
        Assert.Throws<TodoInvalidDescriptionException>(() =>
            new Todo(ValidTitle, TooShortString, TodoPriority.Low, _validUserId));
    }

    [Fact]
    public void Constructor_WithTooLongDescription_ShouldThrow()
    {
        Assert.Throws<TodoInvalidDescriptionException>(() =>
            new Todo(ValidTitle, TooLongDescription, TodoPriority.Low, _validUserId));
    }

    [Fact]
    public void Constructor_WithNullDescription_ShouldCreate()
    {
        var todo = new Todo(ValidTitle, null, TodoPriority.Low, _validUserId);

        Assert.Null(todo.Description);
    }

    [Fact]
    public void Constructor_WithEmptyDescription_ShouldCreate()
    {
        var todo = new Todo(ValidTitle, string.Empty, TodoPriority.Low, _validUserId);

        Assert.Equal(string.Empty, todo.Description);
    }

    // --- UPDATE TESTS ---

    [Fact]
    public void Update_WithValidValues_ShouldUpdateTodo()
    {
        var todo = new Todo(ValidTitle, ValidDescription, TodoPriority.Low, _validUserId);

        todo.Update("Updated", "Updated description", TodoPriority.High);

        Assert.Equal("Updated", todo.Title);
        Assert.Equal("Updated description", todo.Description);
        Assert.Equal(TodoPriority.High, todo.Priority);
    }

    [Fact]
    public void Update_WithTooShortTitle_ShouldThrow()
    {
        var todo = new Todo(ValidTitle, ValidDescription, TodoPriority.Low, _validUserId);

        Assert.Throws<TodoInvalidTitleException>(() =>
            todo.Update(TooShortString, ValidDescription, TodoPriority.High));
    }

    [Fact]
    public void Update_WithTooLongTitle_ShouldThrow()
    {
        var todo = new Todo(ValidTitle, ValidDescription, TodoPriority.Low, _validUserId);

        Assert.Throws<TodoInvalidTitleException>(() =>
            todo.Update(TooLongTitle, ValidDescription, TodoPriority.High));
    }

    [Fact]
    public void Update_WithTooShortDescription_ShouldThrow()
    {
        var todo = new Todo(ValidTitle, ValidDescription, TodoPriority.Low, _validUserId);

        Assert.Throws<TodoInvalidDescriptionException>(() =>
            todo.Update(ValidTitle, TooShortString, TodoPriority.High));
    }

    [Fact]
    public void Update_WithTooLongDescription_ShouldThrow()
    {
        var todo = new Todo(ValidTitle, ValidDescription, TodoPriority.Low, _validUserId);

        Assert.Throws<TodoInvalidDescriptionException>(() =>
            todo.Update(ValidTitle, TooLongDescription, TodoPriority.High));
    }

    [Fact]
    public void Update_WithNullDescription_ShouldUpdate()
    {
        var todo = new Todo(ValidTitle, ValidDescription, TodoPriority.Low, _validUserId);

        todo.Update(ValidTitle, null, TodoPriority.High);

        Assert.Null(todo.Description);
    }

    // --- COMPLETION STATUS ---

    [Fact]
    public void SetCompletionStatus_WhenChanging_ShouldUpdateStatus()
    {
        var todo = new Todo(ValidTitle, ValidDescription, TodoPriority.Medium, _validUserId);

        todo.SetCompletionStatus(true);

        Assert.True(todo.IsCompleted);
    }

    [Fact]
    public void SetCompletionStatus_WhenSettingSameValue_ShouldNotChange()
    {
        var todo = new Todo(ValidTitle, ValidDescription, TodoPriority.Medium, _validUserId);

        todo.SetCompletionStatus(false); // already false

        Assert.False(todo.IsCompleted);
    }

    // --- PRIORITY ---

    [Fact]
    public void ChangePriority_WhenDifferent_ShouldUpdatePriority()
    {
        var todo = new Todo(ValidTitle, ValidDescription, TodoPriority.Low, _validUserId);

        todo.ChangePriority(TodoPriority.High);

        Assert.Equal(TodoPriority.High, todo.Priority);
    }

    [Fact]
    public void ChangePriority_WhenSame_ShouldNotChange()
    {
        var todo = new Todo(ValidTitle, ValidDescription, TodoPriority.Medium, _validUserId);

        todo.ChangePriority(TodoPriority.Medium);

        Assert.Equal(TodoPriority.Medium, todo.Priority);
    }

    // --- NAVIGATION PROPERTIES ---

    [Fact]
    public void Tags_ShouldBeEmptyByDefault()
    {
        var todo = new Todo(ValidTitle, ValidDescription, TodoPriority.Low, _validUserId);

        Assert.Empty(todo.Tags);
    }

    [Fact]
    public void Attachments_ShouldBeEmptyByDefault()
    {
        var todo = new Todo(ValidTitle, ValidDescription, TodoPriority.Low, _validUserId);

        Assert.Empty(todo.Attachments);
    }
}