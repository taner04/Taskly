using Api.Features.Todos.Domain;

namespace UnitTests.Tests;

public class TodoTests
{
    private const string ValidTitle = "Valid Title";
    private const string ValidDescription = "Valid Description";
    private const string ValidUserId = "user123";
    private const string TestTodoTitle = "Test Todo";
    private const string TestDescription = "Test Description";
    private const string UpdatedTitle = "Updated Title";
    private const string UpdatedDescription = "Updated Description";
    private const string InvalidShortString = "ab"; // length = 2 < MinTitle/DescriptionLength

    // ------------------------------------------------------------
    // TryCreate - Happy Path
    // ------------------------------------------------------------

    [Fact]
    public void TryCreate_WithValidData_ShouldReturnTodo()
    {
        var result = Todo.TryCreate(TestTodoTitle, TestDescription, TodoPriority.Medium, ValidUserId);
        
        Assert.False(result.IsError);
        var todo = result.Value;
        Assert.Equal(TestTodoTitle, todo.Title);
        Assert.Equal(TestDescription, todo.Description);
        Assert.Equal(TodoPriority.Medium, todo.Priority);
        Assert.Equal(ValidUserId, todo.UserId);
        Assert.False(todo.IsCompleted);
    }

    // ------------------------------------------------------------
    // TryCreate - Title Validation (Too Short / Too Long / Boundaries)
    // ------------------------------------------------------------

    [Fact]
    public void TryCreate_WithTitleTooShort_ShouldReturnError()
    {
        var result = Todo.TryCreate(InvalidShortString, ValidDescription, TodoPriority.Medium, ValidUserId);

        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Code == "Todo.MaxTitleLength");
    }

    [Fact]
    public void TryCreate_WithTitleTooLong_ShouldReturnError()
    {
        var title = new string('a', Todo.MaxTitleLength + 1);

        var result = Todo.TryCreate(title, ValidDescription, TodoPriority.Medium, ValidUserId);

        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Code == "Todo.MaxTitleLength");
    }

    [Fact]
    public void TryCreate_WithTitleAtMinLength_ShouldReturnSuccess()
    {
        var title = new string('a', Todo.MinTitleLength);

        var result = Todo.TryCreate(title, ValidDescription, TodoPriority.Medium, ValidUserId);

        Assert.False(result.IsError);
        Assert.Equal(title, result.Value.Title);
    }

    [Fact]
    public void TryCreate_WithTitleAtMaxLength_ShouldReturnSuccess()
    {
        var title = new string('a', Todo.MaxTitleLength);

        var result = Todo.TryCreate(title, ValidDescription, TodoPriority.Medium, ValidUserId);

        Assert.False(result.IsError);
        Assert.Equal(title, result.Value.Title);
    }

    // ------------------------------------------------------------
    // TryCreate - Description Validation (Too Short / Too Long / Boundaries)
    // ------------------------------------------------------------

    [Fact]
    public void TryCreate_WithDescriptionTooShort_ShouldReturnError()
    {
        var result = Todo.TryCreate(ValidTitle, InvalidShortString, TodoPriority.Medium, ValidUserId);

        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Code == "Todo.Description");
    }

    [Fact]
    public void TryCreate_WithDescriptionTooLong_ShouldReturnError()
    {
        var description = new string('a', Todo.MaxDescriptionLength + 1);

        var result = Todo.TryCreate(ValidTitle, description, TodoPriority.Medium, ValidUserId);

        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Code == "Todo.Description");
    }

    [Fact]
    public void TryCreate_WithDescriptionAtMinLength_ShouldReturnSuccess()
    {
        var description = new string('a', Todo.MinDescriptionLength);

        var result = Todo.TryCreate(ValidTitle, description, TodoPriority.Medium, ValidUserId);

        Assert.False(result.IsError);
        Assert.Equal(description, result.Value.Description);
    }

    [Fact]
    public void TryCreate_WithDescriptionAtMaxLength_ShouldReturnSuccess()
    {
        var description = new string('a', Todo.MaxDescriptionLength);

        var result = Todo.TryCreate(ValidTitle, description, TodoPriority.Medium, ValidUserId);

        Assert.False(result.IsError);
        Assert.Equal(description, result.Value.Description);
    }

    [Fact]
    public void TryCreate_WithNullDescription_ShouldReturnSuccess()
    {
        string? description = null;

        var result = Todo.TryCreate(ValidTitle, description, TodoPriority.Medium, ValidUserId);

        Assert.False(result.IsError);
        Assert.Null(result.Value.Description);
    }

    [Fact]
    public void TryCreate_WithEmptyDescription_ShouldReturnSuccess()
    {
        var result = Todo.TryCreate(ValidTitle, string.Empty, TodoPriority.Medium, ValidUserId);

        Assert.False(result.IsError);
        Assert.Equal(string.Empty, result.Value.Description);
    }

    // ------------------------------------------------------------
    // TryCreate - UserId Validation
    // ------------------------------------------------------------

    [Fact]
    public void TryCreate_WithEmptyUserId_ShouldReturnError()
    {
        var result = Todo.TryCreate(ValidTitle, ValidDescription, TodoPriority.Medium, "");

        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Code == "Todo.UserId");
    }

    [Fact]
    public void TryCreate_WithNullUserId_ShouldReturnError()
    {
        string userId = null!;

        var result = Todo.TryCreate(ValidTitle, ValidDescription, TodoPriority.Medium, userId);

        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Code == "Todo.UserId");
    }

    // ------------------------------------------------------------
    // Update - Happy Path
    // ------------------------------------------------------------

    [Fact]
    public void Update_WithValidData_ShouldUpdateTodo()
    {
        var createResult = Todo.TryCreate(ValidTitle, ValidDescription, TodoPriority.Low, ValidUserId);
        var todo = createResult.Value;

        var updateResult = todo.Update(UpdatedTitle, UpdatedDescription, TodoPriority.High);

        Assert.False(updateResult.IsError);
        Assert.Equal(UpdatedTitle, todo.Title);
        Assert.Equal(UpdatedDescription, todo.Description);
        Assert.Equal(TodoPriority.High, todo.Priority);
    }

    // ------------------------------------------------------------
    // Update - Error Cases
    // ------------------------------------------------------------

    [Fact]
    public void Update_WithInvalidTitle_ShouldReturnError()
    {
        var createResult = Todo.TryCreate(ValidTitle, ValidDescription, TodoPriority.Low, ValidUserId);
        var todo = createResult.Value;

        var updateResult = todo.Update(InvalidShortString, ValidDescription, TodoPriority.High);

        Assert.True(updateResult.IsError);
        Assert.Contains(updateResult.Errors, e => e.Code == "Todo.MaxTitleLength");
    }

    [Fact]
    public void Update_WithDescriptionTooShort_ShouldReturnError()
    {
        var todo = Todo.TryCreate(ValidTitle, ValidDescription, TodoPriority.Low, ValidUserId).Value;

        var invalidDescription = new string('a', Todo.MinDescriptionLength - 1);

        var updateResult = todo.Update(ValidTitle, invalidDescription, TodoPriority.High);

        Assert.True(updateResult.IsError);
        Assert.Contains(updateResult.Errors, e => e.Code == "Todo.Description");
    }

    [Fact]
    public void Update_WithDescriptionTooLong_ShouldReturnError()
    {
        var todo = Todo.TryCreate(ValidTitle, ValidDescription, TodoPriority.Low, ValidUserId).Value;

        var invalidDescription = new string('a', Todo.MaxDescriptionLength + 1);

        var updateResult = todo.Update(ValidTitle, invalidDescription, TodoPriority.High);

        Assert.True(updateResult.IsError);
        Assert.Contains(updateResult.Errors, e => e.Code == "Todo.Description");
    }

    [Fact]
    public void Update_WithNullDescription_ShouldReturnSuccess()
    {
        var todo = Todo.TryCreate(ValidTitle, ValidDescription, TodoPriority.Low, ValidUserId).Value;

        var updateResult = todo.Update(UpdatedTitle, null, TodoPriority.High);

        Assert.False(updateResult.IsError);
        Assert.Null(todo.Description);
    }

    // ------------------------------------------------------------
    // SetCompletionStatus Tests
    // ------------------------------------------------------------

    [Fact]
    public void SetCompletionStatus_FromFalseToTrue_ShouldSetCompleted()
    {
        var todo = Todo.TryCreate(ValidTitle, ValidDescription, TodoPriority.Medium, ValidUserId).Value;

        todo.SetCompletionStatus(true);

        Assert.True(todo.IsCompleted);
    }

    [Fact]
    public void SetCompletionStatus_FromTrueToFalse_ShouldSetNotCompleted()
    {
        var todo = Todo.TryCreate(ValidTitle, ValidDescription, TodoPriority.Medium, ValidUserId).Value;
        todo.SetCompletionStatus(true);

        todo.SetCompletionStatus(false);

        Assert.False(todo.IsCompleted);
    }

    [Fact]
    public void SetCompletionStatus_WithSameValue_ShouldNotChange()
    {
        var todo = Todo.TryCreate(ValidTitle, ValidDescription, TodoPriority.Medium, ValidUserId).Value;

        todo.SetCompletionStatus(false); // already false
        todo.SetCompletionStatus(false);

        Assert.False(todo.IsCompleted);
    }

    // ------------------------------------------------------------
    // ChangePriority Tests
    // ------------------------------------------------------------

    [Fact]
    public void ChangePriority_WithDifferentPriority_ShouldUpdatePriority()
    {
        var todo = Todo.TryCreate(ValidTitle, ValidDescription, TodoPriority.Low, ValidUserId).Value;

        todo.ChangePriority(TodoPriority.High);

        Assert.Equal(TodoPriority.High, todo.Priority);
    }

    [Fact]
    public void ChangePriority_WithSamePriority_ShouldNotChange()
    {
        var todo = Todo.TryCreate(ValidTitle, ValidDescription, TodoPriority.Medium, ValidUserId).Value;

        todo.ChangePriority(TodoPriority.Medium);

        Assert.Equal(TodoPriority.Medium, todo.Priority);
    }

    // ------------------------------------------------------------
    // TryCreate with Priority Variations
    // ------------------------------------------------------------

    [Theory]
    [InlineData(TodoPriority.Low)]
    [InlineData(TodoPriority.Medium)]
    [InlineData(TodoPriority.High)]
    public void TryCreate_WithDifferentPriorities_ShouldReturnSuccess(TodoPriority priority)
    {
        var result = Todo.TryCreate(TestTodoTitle, TestDescription, priority, ValidUserId);

        Assert.False(result.IsError);
        Assert.Equal(priority, result.Value.Priority);
    }
}