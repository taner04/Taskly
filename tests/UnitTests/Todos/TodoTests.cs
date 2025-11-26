using Api.Features.Todos.Domain;

namespace UnitTests.Todos;

public class TodoTests
{
    private const string ValidTitle = "Valid Title";
    private const string ValidDescription = "Valid Description";
    private const string ValidUserId = "user123";
    private const string TestTodoTitle = "Test Todo";
    private const string TestDescription = "Test Description";
    private const string OriginalTitle = "Original Title";
    private const string OriginalDescription = "Original Description";
    private const string UpdatedTitle = "Updated Title";
    private const string UpdatedDescription = "Updated Description";
    private const string InvalidShortString = "ab";

    [Fact]
    public void TryCreate_WithValidData_ShouldReturnTodo()
    {
        // Arrange
        var priority = TodoPriority.Medium;

        // Act
        var result = Todo.TryCreate(TestTodoTitle, TestDescription, priority, ValidUserId);

        // Assert
        Assert.False(result.IsError);
        var todo = result.Value;
        Assert.Equal(TestTodoTitle, todo.Title);
        Assert.Equal(TestDescription, todo.Description);
        Assert.Equal(priority, todo.Priority);
        Assert.Equal(ValidUserId, todo.UserId);
        Assert.False(todo.IsCompleted);
    }

    [Fact]
    public void TryCreate_WithTitleTooShort_ShouldReturnError()
    {
        // Arrange
        var priority = TodoPriority.Medium;

        // Act
        var result = Todo.TryCreate(InvalidShortString, ValidDescription, priority, ValidUserId);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Code == "Todo.MaxTitleLength");
    }

    [Fact]
    public void TryCreate_WithTitleTooLong_ShouldReturnError()
    {
        // Arrange
        var title = new string('a', Todo.MaxTitleLength + 1);
        var priority = TodoPriority.Medium;

        // Act
        var result = Todo.TryCreate(title, ValidDescription, priority, ValidUserId);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Code == "Todo.MaxTitleLength");
    }

    [Fact]
    public void TryCreate_WithDescriptionTooShort_ShouldReturnError()
    {
        // Arrange
        var priority = TodoPriority.Medium;

        // Act
        var result = Todo.TryCreate(ValidTitle, InvalidShortString, priority, ValidUserId);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Code == "Todo.Description");
    }

    [Fact]
    public void TryCreate_WithDescriptionTooLong_ShouldReturnError()
    {
        // Arrange
        var description = new string('a', Todo.MaxDescriptionLength + 1);
        var priority = TodoPriority.Medium;

        // Act
        var result = Todo.TryCreate(ValidTitle, description, priority, ValidUserId);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Code == "Todo.Description");
    }

    [Fact]
    public void TryCreate_WithNullDescription_ShouldReturnSuccess()
    {
        // Arrange
        string? description = null;
        var priority = TodoPriority.Medium;

        // Act
        var result = Todo.TryCreate(ValidTitle, description, priority, ValidUserId);

        // Assert
        Assert.False(result.IsError);
        var todo = result.Value;
        Assert.Null(todo.Description);
    }

    [Fact]
    public void TryCreate_WithEmptyUserId_ShouldReturnError()
    {
        // Arrange
        var priority = TodoPriority.Medium;
        var userId = "";

        // Act
        var result = Todo.TryCreate(ValidTitle, ValidDescription, priority, userId);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Code == "Todo.UserId");
    }

    [Fact]
    public void TryCreate_WithNullUserId_ShouldReturnError()
    {
        // Arrange
        var priority = TodoPriority.Medium;
        string userId = null!;

        // Act
        var result = Todo.TryCreate(ValidTitle, ValidDescription, priority, userId);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Code == "Todo.UserId");
    }

    [Fact]
    public void Update_WithValidData_ShouldUpdateTodo()
    {
        // Arrange
        var result = Todo.TryCreate(OriginalTitle, OriginalDescription, TodoPriority.Low, ValidUserId);
        var todo = result.Value;
        var newPriority = TodoPriority.High;

        // Act
        var updateResult = todo.Update(UpdatedTitle, UpdatedDescription, newPriority);

        // Assert
        Assert.False(updateResult.IsError);
        Assert.Equal(UpdatedTitle, todo.Title);
        Assert.Equal(UpdatedDescription, todo.Description);
        Assert.Equal(newPriority, todo.Priority);
        Assert.False(todo.IsCompleted);
    }

    [Fact]
    public void Update_WithInvalidTitle_ShouldReturnError()
    {
        // Arrange
        var result = Todo.TryCreate(ValidTitle, ValidDescription, TodoPriority.Low, ValidUserId);
        var todo = result.Value;

        // Act
        var updateResult = todo.Update(InvalidShortString, ValidDescription, TodoPriority.High);

        // Assert
        Assert.True(updateResult.IsError);
        Assert.Contains(updateResult.Errors, e => e.Code == "Todo.MaxTitleLength");
    }

    [Fact]
    public void Update_WithNullDescription_ShouldReturnSuccess()
    {
        // Arrange
        var result = Todo.TryCreate(ValidTitle, ValidDescription, TodoPriority.Low, ValidUserId);
        var todo = result.Value;

        // Act
        var updateResult = todo.Update(UpdatedTitle, null, TodoPriority.High);

        // Assert
        Assert.False(updateResult.IsError);
        Assert.Null(todo.Description);
    }

    [Fact]
    public void ChangePriority_WithDifferentPriority_ShouldChangePriority()
    {
        // Arrange
        var result = Todo.TryCreate(ValidTitle, ValidDescription, TodoPriority.Low, ValidUserId);
        var todo = result.Value;
        var newPriority = TodoPriority.High;

        // Act
        todo.ChangePriority(newPriority);

        // Assert
        Assert.Equal(newPriority, todo.Priority);
    }

    [Fact]
    public void ChangePriority_WithSamePriority_ShouldNotChange()
    {
        // Arrange
        var result = Todo.TryCreate(ValidTitle, ValidDescription, TodoPriority.Medium, ValidUserId);
        var todo = result.Value;
        var samePriority = TodoPriority.Medium;

        // Act
        todo.ChangePriority(samePriority);

        // Assert
        Assert.Equal(samePriority, todo.Priority);
    }

    [Theory]
    [InlineData(TodoPriority.Low)]
    [InlineData(TodoPriority.Medium)]
    [InlineData(TodoPriority.High)]
    public void TryCreate_WithDifferentPriorities_ShouldReturnSuccess(TodoPriority priority)
    {
        // Act
        var result = Todo.TryCreate(TestTodoTitle, TestDescription, priority, ValidUserId);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal(priority, result.Value.Priority);
    }
}