using System.Net;
using Api.Features.Todos.Endpoints;
using Api.Features.Todos.Model;
using FluentAssertions;
using IntegrationTests.Extensions;

namespace IntegrationTests.Tests.Todos;

public sealed class CompleteTodoTests(TestingFixture fixture) : TestingBase(fixture)
{
    private static Todo CreateTodo(
        UserId userId,
        bool completed = false)
    {
        var todo = Todo.Create(
            "Test Todo",
            "Test Description",
            TodoPriority.Medium,
            userId);

        todo.SetCompletionStatus(completed);
        return todo;
    }

    [Fact]
    public async Task CompleteTodo_Should_Return401_When_Unauthenticated()
    {
        // Arrange
        var client = CreateUnauthenticatedClient();
        var todoId = TodoId.From(Guid.NewGuid());

        // Act
        var response = await client.CompleteTodoAsync(
            todoId,
            new CompleteTodo.Command.CommandBody
            {
                Completed = true
            },
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CompleteTodo_Should_Return404_When_TodoDoesNotExist()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var todoId = TodoId.From(Guid.NewGuid());

        // Act
        var response = await client.CompleteTodoAsync(
            todoId,
            new CompleteTodo.Command.CommandBody
            {
                Completed = false
            },
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        await response.ContainsErrorCode("Todo.NotFound", CurrentCancellationToken);
    }

    [Fact]
    public async Task CompleteTodo_Should_SetCompletedTrue()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var userId = GetCurrentUserId();
        var todo = CreateTodo(userId);

        DbContext.Add(todo);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        // Act
        var response = await client.CompleteTodoAsync(
            todo.Id,
            new CompleteTodo.Command.CommandBody
            {
                Completed = true
            },
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updated = await DbContext.Todos
            .AsNoTracking()
            .FirstAsync(t => t.Id == todo.Id, CurrentCancellationToken);

        updated.IsCompleted.Should().BeTrue();
    }

    [Fact]
    public async Task CompleteTodo_Should_SetCompletedFalse()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var userId = GetCurrentUserId();
        var todo = CreateTodo(userId, true);

        DbContext.Add(todo);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        // Act
        var response = await client.CompleteTodoAsync(
            todo.Id,
            new CompleteTodo.Command.CommandBody
            {
                Completed = false
            },
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updated = await DbContext.Todos
            .AsNoTracking()
            .FirstAsync(t => t.Id == todo.Id, CurrentCancellationToken);

        updated.IsCompleted.Should().BeFalse();
    }

    [Fact]
    public async Task CompleteTodo_Should_NotAffect_Todos_From_OtherUsers()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var foreignTodo = CreateTodo(UserId.EmptyId);

        DbContext.Add(foreignTodo);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        // Act
        var response = await client.CompleteTodoAsync(
            foreignTodo.Id,
            new CompleteTodo.Command.CommandBody
            {
                Completed = true
            },
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        await response.ContainsErrorCode("Todo.NotFound", CurrentCancellationToken);

        var unchanged = await DbContext.Todos
            .AsNoTracking()
            .FirstAsync(t => t.Id == foreignTodo.Id, CurrentCancellationToken);

        unchanged.IsCompleted.Should().BeFalse();
    }
}