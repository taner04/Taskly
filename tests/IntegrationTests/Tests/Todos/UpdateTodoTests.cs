using System.Net;
using Api.Features.Todos.Endpoints;
using Api.Features.Todos.Model;
using FluentAssertions;
using IntegrationTests.Extensions;

namespace IntegrationTests.Tests.Todos;

public sealed class UpdateTodoTests(TestingFixture fixture) : TestingBase(fixture)
{
    private static Todo CreateTodo(
        UserId userId)
    {
        return Todo.Create(
            "Original Title",
            "Original Description",
            TodoPriority.Medium,
            userId);
    }

    [Fact]
    public async Task UpdateTodo_Should_Return401_When_Unauthenticated()
    {
        // Arrange
        var client = CreateUnauthenticatedClient();

        // Act
        var response = await client.UpdateTodoAsync(
            TodoId.From(Guid.NewGuid()),
            new UpdateTodo.Command.CommandBody
            {
                Title = "Updated",
                Description = "Updated Desc",
                Priority = TodoPriority.Low
            },
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateTodo_Should_Return404_When_TodoDoesNotExist()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();

        // Act
        var response = await client.UpdateTodoAsync(
            TodoId.From(Guid.NewGuid()),
            new UpdateTodo.Command.CommandBody
            {
                Title = "Updated",
                Description = "Updated Desc",
                Priority = TodoPriority.High
            },
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        await response.ContainsErrorCode("Todo.NotFound", CurrentCancellationToken);
    }

    [Fact]
    public async Task UpdateTodo_Should_Return400_When_TitleIsInvalid()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        var userId = CurrentUserId;
        var todo = CreateTodo(userId);

        await using var dbContext = GetDbContext();
        dbContext.Add(todo);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        // Act
        var response = await client.UpdateTodoAsync(
            todo.Id,
            new UpdateTodo.Command.CommandBody
            {
                Title = "Hi", // too short
                Description = "Valid Desc",
                Priority = TodoPriority.High
            },
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        await response.ContainsErrorCode("Todo.InvalidTitle", CurrentCancellationToken);
    }

    [Fact]
    public async Task UpdateTodo_Should_Return400_When_DescriptionIsInvalid()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        var userId = CurrentUserId;
        var todo = CreateTodo(userId);

        await using var dbContext = GetDbContext();
        dbContext.Add(todo);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        // Act
        var response = await client.UpdateTodoAsync(
            todo.Id,
            new UpdateTodo.Command.CommandBody
            {
                Title = "Valid Title",
                Description = "Hi", // invalid
                Priority = TodoPriority.Low
            },
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        await response.ContainsErrorCode("Todo.InvalidDescription", CurrentCancellationToken);
    }

    [Fact]
    public async Task UpdateTodo_Should_Return200_And_UpdateTodo()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        var userId = CurrentUserId;
        var todo = CreateTodo(userId);

        await using var dbContext = GetDbContext();
        dbContext.Add(todo);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        var body = new UpdateTodo.Command.CommandBody
        {
            Title = "Updated Title",
            Description = "Updated Description",
            Priority = TodoPriority.High
        };

        // Act
        var response = await client.UpdateTodoAsync(
            todo.Id,
            body,
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updated = await GetDbContext().Todos
            .AsNoTracking()
            .FirstAsync(t => t.Id == todo.Id, CurrentCancellationToken);

        updated.Title.Should().Be(body.Title);
        updated.Description.Should().Be(body.Description);
        updated.Priority.Should().Be(body.Priority);
    }

    [Fact]
    public async Task UpdateTodo_Should_NotUpdate_Todos_From_OtherUsers()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();

        var foreignUserId = await CreateForeignUserAsync();
        var foreignTodo = CreateTodo(foreignUserId);

        await using var dbContext = GetDbContext();
        dbContext.Add(foreignTodo);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        var body = new UpdateTodo.Command.CommandBody
        {
            Title = "Should Not Update",
            Description = "Forbidden",
            Priority = TodoPriority.Low
        };

        // Act
        var response = await client.UpdateTodoAsync(
            foreignTodo.Id,
            body,
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        await response.ContainsErrorCode("Todo.NotFound", CurrentCancellationToken);

        var untouched = await GetDbContext().Todos
            .AsNoTracking()
            .FirstAsync(t => t.Id == foreignTodo.Id, CurrentCancellationToken);

        untouched.Title.Should().Be("Original Title");
        untouched.Description.Should().Be("Original Description");
        untouched.Priority.Should().Be(TodoPriority.Medium);
    }
}