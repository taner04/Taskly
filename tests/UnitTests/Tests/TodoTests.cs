using Api.Features.Shared.Exceptions;
using Api.Features.Todos.Exceptions;
using Api.Features.Todos.Model;
using Api.Features.Users.Model;

namespace UnitTests.Tests;

public sealed class TodoTests
{
    private const string ValidTitle = "Valid Todo";
    private const string ValidDescription = "Valid description";

    private const string TooShortString = "ab";
    private static readonly string TooLongTitle = new('x', Todo.MaxTitleLength + 1);
    private static readonly string TooLongDescription = new('x', Todo.MaxDescriptionLength + 1);
    private readonly UserId _validUserId = UserId.From(Guid.Parse("00000000-0000-0000-0000-000000000001"));

    private Todo CreateTodo()
    {
        return Todo.Create(
            ValidTitle,
            ValidDescription,
            TodoPriority.Medium,
            _validUserId);
    }

    [Fact]
    public void Create_WithValidData_ShouldCreateTodo()
    {
        var todo = CreateTodo();

        Assert.Equal(ValidTitle, todo.Title);
        Assert.Equal(ValidDescription, todo.Description);
        Assert.Equal(TodoPriority.Medium, todo.Priority);
        Assert.False(todo.IsCompleted);
        Assert.Equal(_validUserId, todo.UserId);
        Assert.Null(todo.Deadline);
        Assert.Null(todo.ReminderOffsetInMinutes);
        Assert.Null(todo.ReminderAt);
        Assert.NotEqual(Guid.Empty, todo.Id.Value);
    }

    [Fact]
    public void Create_WithTooShortTitle_ShouldThrow()
    {
        Assert.Throws<ModelInvalidStringException<Todo>>(() =>
            Todo.Create(TooShortString, ValidDescription, TodoPriority.Low, _validUserId));
    }

    [Fact]
    public void Create_WithTooLongTitle_ShouldThrow()
    {
        Assert.Throws<ModelInvalidStringException<Todo>>(() =>
            Todo.Create(TooLongTitle, ValidDescription, TodoPriority.Low, _validUserId));
    }

    [Fact]
    public void Create_WithTitleAtMinLength_ShouldCreate()
    {
        var title = new string('a', Todo.MinTitleLength);
        var todo = Todo.Create(title, ValidDescription, TodoPriority.Low, _validUserId);
        Assert.Equal(title, todo.Title);
    }

    [Fact]
    public void Create_WithTitleAtMaxLength_ShouldCreate()
    {
        var title = new string('a', Todo.MaxTitleLength);
        var todo = Todo.Create(title, ValidDescription, TodoPriority.Low, _validUserId);
        Assert.Equal(title, todo.Title);
    }

    [Fact]
    public void Create_WithTooShortDescription_ShouldThrow()
    {
        Assert.Throws<ModelInvalidStringException<Todo>>(() =>
            Todo.Create(ValidTitle, TooShortString, TodoPriority.Low, _validUserId));
    }

    [Fact]
    public void Create_WithTooLongDescription_ShouldThrow()
    {
        Assert.Throws<ModelInvalidStringException<Todo>>(() =>
            Todo.Create(ValidTitle, TooLongDescription, TodoPriority.Low, _validUserId));
    }

    [Fact]
    public void Create_WithNullDescription_ShouldCreate()
    {
        var todo = Todo.Create(ValidTitle, null, TodoPriority.Low, _validUserId);
        Assert.Null(todo.Description);
    }

    [Fact]
    public void Create_WithEmptyDescription_ShouldCreate()
    {
        var todo = Todo.Create(ValidTitle, string.Empty, TodoPriority.Low, _validUserId);
        Assert.Equal(string.Empty, todo.Description);
    }

    [Fact]
    public void Update_WithValidValues_ShouldUpdateTodo()
    {
        var todo = CreateTodo();
        todo.Update("Updated", "Updated description", TodoPriority.High);

        Assert.Equal("Updated", todo.Title);
        Assert.Equal("Updated description", todo.Description);
        Assert.Equal(TodoPriority.High, todo.Priority);
    }

    [Fact]
    public void Update_WithTooShortTitle_ShouldThrow()
    {
        var todo = CreateTodo();
        Assert.Throws<ModelInvalidStringException<Todo>>(() =>
            todo.Update(TooShortString, ValidDescription, TodoPriority.High));
    }

    [Fact]
    public void Update_WithTooLongTitle_ShouldThrow()
    {
        var todo = CreateTodo();
        Assert.Throws<ModelInvalidStringException<Todo>>(() =>
            todo.Update(TooLongTitle, ValidDescription, TodoPriority.High));
    }

    [Fact]
    public void Update_WithTooShortDescription_ShouldThrow()
    {
        var todo = CreateTodo();
        Assert.Throws<ModelInvalidStringException<Todo>>(() =>
            todo.Update(ValidTitle, TooShortString, TodoPriority.High));
    }

    [Fact]
    public void Update_WithTooLongDescription_ShouldThrow()
    {
        var todo = CreateTodo();
        Assert.Throws<ModelInvalidStringException<Todo>>(() =>
            todo.Update(ValidTitle, TooLongDescription, TodoPriority.High));
    }

    [Fact]
    public void Update_WithNullDescription_ShouldUpdateToNull()
    {
        var todo = CreateTodo();
        todo.Update(ValidTitle, null, TodoPriority.High);
        Assert.Null(todo.Description);
    }

    [Fact]
    public void SetCompletionStatus_ShouldUpdate()
    {
        var todo = CreateTodo();
        todo.SetCompletionStatus(true);
        Assert.True(todo.IsCompleted);
    }

    [Fact]
    public void ChangePriority_ShouldUpdate()
    {
        var todo = CreateTodo();
        todo.ChangePriority(TodoPriority.High);
        Assert.Equal(TodoPriority.High, todo.Priority);
    }

    [Fact]
    public void SetReminder_WithValidValues_ShouldSetFields()
    {
        var todo = CreateTodo();
        var deadline = DateTime.UtcNow.AddHours(3);

        todo.SetReminder(deadline, 30);

        Assert.Equal(deadline, todo.Deadline);
        Assert.Equal(30, todo.ReminderOffsetInMinutes);
        Assert.Equal(deadline.AddMinutes(-30), todo.ReminderAt);
    }

    [Fact]
    public void SetReminder_WithPastDeadline_ShouldThrow()
    {
        var todo = CreateTodo();
        var past = DateTime.UtcNow.AddHours(-1);

        Assert.Throws<TodoInvalidDeadlineException>(() =>
            todo.SetReminder(past, 30));
    }

    [Fact]
    public void SetReminder_WithNegativeOffset_ShouldThrow()
    {
        var todo = CreateTodo();
        var deadline = DateTime.UtcNow.AddHours(2);

        Assert.Throws<TodoInvalidDeadlineException>(() =>
            todo.SetReminder(deadline, -5));
    }

    [Fact]
    public void SetReminder_WhenOffsetTooLarge_ShouldThrow()
    {
        var todo = CreateTodo();
        var deadline = DateTime.UtcNow.AddMinutes(20);

        Assert.Throws<TodoInvalidDeadlineException>(() =>
            todo.SetReminder(deadline, 999));
    }

    [Fact]
    public void ClearReminder_ShouldResetDeadlineAndOffset()
    {
        var todo = CreateTodo();
        var deadline = DateTime.UtcNow.AddHours(3);

        todo.SetReminder(deadline, 30);
        todo.ClearReminder();

        Assert.Null(todo.Deadline);
        Assert.Null(todo.ReminderOffsetInMinutes);
        Assert.Null(todo.ReminderAt);
    }
}