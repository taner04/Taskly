using System.Net;
using Api.Features.Todos.Model;
using FluentAssertions;
using IntegrationTests.Extensions;

namespace IntegrationTests.Tests.Todos;

public sealed class RemoveTodoTests(TestingFixture fixture) : TestingBase(fixture)
{
    private static Todo CreateTodo(UserId userId)
    {
        return Todo.Create(
            "Test Todo",
            "Test Description",
            TodoPriority.Medium,
            userId);
    }

    [Fact]
    public async Task RemoveTodo_Should_Return401_When_Unauthenticated()
    {
        // Arrange
        var client = CreateUnauthenticatedClient();
        var todoId = TodoId.From(Guid.NewGuid());

        // Act
        var response = await client.RemoveTodoAsync(
            todoId,
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RemoveTodo_Should_Return404_When_TodoDoesNotExist()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var todoId = TodoId.From(Guid.NewGuid());

        // Act
        var response = await client.RemoveTodoAsync(
            todoId,
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        await response.ContainsErrorCode("Todo.NotFound", CurrentCancellationToken);
    }

    [Fact]
    public async Task RemoveTodo_Should_Return200_And_RemoveTodo()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var userId = GetCurrentUserId();

        var todo = CreateTodo(userId);

        DbContext.Add(todo);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        // Act
        var response = await client.RemoveTodoAsync(
            todo.Id,
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var exists = await DbContext.Todos
            .AnyAsync(t => t.Id == todo.Id, CurrentCancellationToken);

        exists.Should().BeFalse();
    }

    [Fact]
    public async Task RemoveTodo_Should_NotRemove_Todos_From_OtherUsers()
    {
        // Arrange
        var client = CreateAuthenticatedClient();

        var foreignTodo = CreateTodo(UserId.EmptyId);

        DbContext.Add(foreignTodo);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        // Act
        var response = await client.RemoveTodoAsync(
            foreignTodo.Id,
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        await response.ContainsErrorCode("Todo.NotFound", CurrentCancellationToken);

        var stillExists = await DbContext.Todos
            .AnyAsync(t => t.Id == foreignTodo.Id, CurrentCancellationToken);

        stillExists.Should().BeTrue();
    }
}