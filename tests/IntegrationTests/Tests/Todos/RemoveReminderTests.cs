using System.Net;
using Api.Features.Todos.Model;
using FluentAssertions;

namespace IntegrationTests.Tests.Todos;

public sealed class RemoveReminderTests(TestingFixture fixture) : TestingBase(fixture)
{
    private static Todo CreateTodoWithReminder(
        UserId userId)
    {
        var todo = Todo.Create(
            "Test Todo",
            "Test Description",
            TodoPriority.Medium,
            userId);

        var deadline = DateTime.UtcNow.AddHours(2);
        todo.SetReminder(deadline, 30);

        return todo;
    }

    [Fact]
    public async Task RemoveReminder_Should_Return401_When_Unauthenticated()
    {
        // Arrange
        var client = CreateUnauthenticatedClient();
        var todoId = TodoId.From(Guid.NewGuid());

        // Act
        var response = await client.RemoveReminderAsync(
            todoId,
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RemoveReminder_Should_Return404_When_TodoDoesNotExist()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        var todoId = TodoId.From(Guid.NewGuid());

        // Act
        var response = await client.RemoveReminderAsync(
            todoId,
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        await response.ContainsErrorCode("Todo.NotFound", CurrentCancellationToken);
    }

    [Fact]
    public async Task RemoveReminder_Should_Return204_And_ClearDeadline_And_ReminderOffset()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        var userId = CurrentUserId;

        var todo = CreateTodoWithReminder(userId);

        await using var dbContext = GetDbContext();
        dbContext.Add(todo);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        // Act
        var response = await client.RemoveReminderAsync(
            todo.Id,
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updated = await GetDbContext().Todos
            .AsNoTracking()
            .FirstAsync(t => t.Id == todo.Id, CurrentCancellationToken);

        updated.Deadline.Should().BeNull();
        updated.ReminderOffsetInMinutes.Should().BeNull();
        updated.ReminderAt.Should().BeNull();
    }

    [Fact]
    public async Task RemoveReminder_Should_NotAllow_Removing_Reminder_From_OtherUsers_Todo()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();

        var foreignUserId = await CreateForeignUserAsync();
        var foreignTodo = Todo.Create(
            "Test",
            "Desc",
            TodoPriority.Low,
            foreignUserId);

        var deadline = DateTime.UtcNow.AddHours(5);
        foreignTodo.SetReminder(deadline, 60);

        await using var dbContext = GetDbContext();
        dbContext.Add(foreignTodo);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        // Act
        var response = await client.RemoveReminderAsync(
            foreignTodo.Id,
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        await response.ContainsErrorCode("Todo.NotFound", CurrentCancellationToken);

        var unchanged = await GetDbContext().Todos
            .AsNoTracking()
            .FirstAsync(t => t.Id == foreignTodo.Id, CurrentCancellationToken);

        unchanged.Deadline.Should().NotBeNull();
        unchanged.ReminderOffsetInMinutes.Should().NotBeNull();
        unchanged.ReminderAt.Should().NotBeNull();
    }
}