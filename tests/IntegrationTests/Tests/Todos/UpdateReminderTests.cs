using System.Net;
using Api.Features.Todos.Endpoints;
using Api.Features.Todos.Model;
using FluentAssertions;
using IntegrationTests.Extensions;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.Tests.Todos;

public sealed class UpdateReminderTests(TestingFixture fixture) : TestingBase(fixture)
{
    private static Todo CreateTodo(UserId userId)
    {
        return Todo.Create(
            "Test Todo",
            "Description",
            TodoPriority.Medium,
            userId);
    }

    [Fact]
    public async Task UpdateReminder_Should_Return401_When_Unauthenticated()
    {
        // Arrange
        var client = CreateUnauthenticatedClient();
        var todoId = TodoId.From(Guid.NewGuid());

        // Act
        var response = await client.UpdateReminderAsync(
            todoId,
            new UpdateReminder.Command.CommandBody
            {
                Date = DateTime.Parse("2025-01-01T10:00:00Z"),
                ReminderOffsetInMinutes = 30
            },
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateReminder_Should_Return404_When_TodoDoesNotExist()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var todoId = TodoId.From(Guid.NewGuid());

        // Act
        var response = await client.UpdateReminderAsync(
            todoId,
            new UpdateReminder.Command.CommandBody
            {
                Date = DateTime.Parse("2025-01-01T10:00:00Z"),
                ReminderOffsetInMinutes = 30
            },
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        await response.ContainsErrorCode("Todo.NotFound", CurrentCancellationToken);
    }

    [Fact]
    public async Task UpdateReminder_Should_Return400_When_Date_IsInvalid()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var userId = GetCurrentUserId();
        var todo = CreateTodo(userId);

        DbContext.Add(todo);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        // Act
        var response = await client.UpdateReminderAsync(
            todo.Id,
            new UpdateReminder.Command.CommandBody
            {
                // DateTime.MinValue (UTC) is treated as invalid input by your domain
                Date = DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc),
                ReminderOffsetInMinutes = 10
            },
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        await response.ContainsErrorCode("Todo.InvalidDeadline", CurrentCancellationToken);
    }

    [Fact]
    public async Task UpdateReminder_Should_Return400_When_DeadlineIsInThePast()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var userId = GetCurrentUserId();

        var todo = CreateTodo(userId);

        DbContext.Add(todo);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var pastDate = DateTime.UtcNow.AddMinutes(-10);

        // Act
        var response = await client.UpdateReminderAsync(
            todo.Id,
            new UpdateReminder.Command.CommandBody
            {
                Date = pastDate,
                ReminderOffsetInMinutes = 5
            },
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        await response.ContainsErrorCode("Todo.InvalidDeadline", CurrentCancellationToken);
    }

    [Fact]
    public async Task UpdateReminder_Should_Return400_When_ReminderOffset_IsNegative()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var userId = GetCurrentUserId();
        var todo = CreateTodo(userId);

        DbContext.Add(todo);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var futureDate = DateTime.UtcNow.AddHours(2);

        // Act
        var response = await client.UpdateReminderAsync(
            todo.Id,
            new UpdateReminder.Command.CommandBody
            {
                Date = futureDate,
                ReminderOffsetInMinutes = -5
            },
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        await response.ContainsErrorCode("Todo.InvalidDeadline", CurrentCancellationToken);
    }

    [Fact]
    public async Task UpdateReminder_Should_Return400_When_ReminderOccursAfterDeadline()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var userId = GetCurrentUserId();
        var todo = CreateTodo(userId);

        DbContext.Add(todo);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var futureDate = DateTime.UtcNow.AddHours(1);

        // reminder > time until deadline → invalid
        var offset = 200;

        // Act
        var response = await client.UpdateReminderAsync(
            todo.Id,
            new UpdateReminder.Command.CommandBody
            {
                Date = futureDate,
                ReminderOffsetInMinutes = offset
            },
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        await response.ContainsErrorCode("Todo.InvalidDeadline", CurrentCancellationToken);
    }

    [Fact]
    public async Task UpdateReminder_Should_Return200_And_UpdateReminderValues()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var userId = GetCurrentUserId();
        var todo = CreateTodo(userId);

        DbContext.Add(todo);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var deadline = DateTime.UtcNow.AddHours(3);

        var body = new UpdateReminder.Command.CommandBody
        {
            Date = deadline,
            ReminderOffsetInMinutes = 60
        };

        // Act
        var response = await client.UpdateReminderAsync(
            todo.Id,
            body,
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updated = await DbContext.Todos
            .AsNoTracking()
            .FirstAsync(t => t.Id == todo.Id, CurrentCancellationToken);

        updated.Deadline.Should().BeCloseTo(deadline, TimeSpan.FromSeconds(1));
        updated.ReminderOffsetInMinutes.Should().Be(60);

        updated.ReminderAt.Should().BeCloseTo(deadline.AddMinutes(-60), TimeSpan.FromSeconds(1));
    }
}
