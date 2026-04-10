using System.Net;
using FluentAssertions;
using Taskly.IntegrationTests.Extensions;
using Taskly.IntegrationTests.Infrastructure;
using Taskly.IntegrationTests.Infrastructure.Fixtures;
using Taskly.WebApi.Features.Tags.Models;
using Taskly.WebApi.Features.Todos.Models;
using TagId = Taskly.WebApi.Features.Tags.Models.TagId;
using TodoId = Taskly.WebApi.Features.Todos.Models.TodoId;
using UserId = Taskly.WebApi.Features.Users.Models.UserId;

namespace Taskly.IntegrationTests.Tests.Todos;

public sealed class RemoveTagTests(TestingFixture fixture) : TestingBase(fixture)
{
    private static Todo CreateTodo(
        UserId userId) =>
        Todo.Create(
            "Test Todo",
            "Test Description",
            TodoPriority.Medium,
            userId);

    private static Tag CreateTag(string name, UserId userId) => new(name, userId);

    [Fact]
    public async Task RemoveTag_Should_Return401_When_Unauthenticated()
    {
        // Arrange
        var client = CreateUnauthenticatedClient();
        var todoId = TodoId.From(Guid.NewGuid());
        var tagId = TagId.From(Guid.NewGuid());

        // Act
        var response = await client.RemoveTagFromTodoAsync(
            todoId,
            tagId,
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RemoveTag_Should_Return404_When_TodoDoesNotExist()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        var todoId = TodoId.From(Guid.NewGuid());
        var tagId = TagId.From(Guid.NewGuid());

        // Act
        var response = await client.RemoveTagFromTodoAsync(
            todoId,
            tagId,
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        await response.ContainsErrorCode("Todo.NotFound", CurrentCancellationToken);
    }

    [Fact]
    public async Task RemoveTag_Should_Return404_When_TagNotAssignedToTodo()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        var userId = CurrentUserId;

        var todo = CreateTodo(userId);
        var existingTag = CreateTag("Existing", userId);

        await using var dbContext = GetDbContext();
        dbContext.Add(todo);
        dbContext.Add(existingTag);

        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        // Act
        var response = await client.RemoveTagFromTodoAsync(
            todo.Id,
            TagId.From(Guid.NewGuid()),
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        await response.ContainsErrorCode("Tag.NotFound", CurrentCancellationToken);
    }

    [Fact]
    public async Task RemoveTag_Should_Return200_And_RemoveTagFromTodo()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        var userId = CurrentUserId;

        var todo = CreateTodo(userId);
        var tag = CreateTag("MyTag", userId);

        todo.Tags.Add(tag);

        await using var dbContext = GetDbContext();
        dbContext.Add(tag);
        dbContext.Add(todo);

        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        // Act
        var response = await client.RemoveTagFromTodoAsync(
            todo.Id,
            tag.Id,
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updated = await GetDbContext().Todos
            .Include(t => t.Tags)
            .AsNoTracking()
            .FirstAsync(t => t.Id == todo.Id, CurrentCancellationToken);

        updated.Tags.Should().BeEmpty();
    }
}