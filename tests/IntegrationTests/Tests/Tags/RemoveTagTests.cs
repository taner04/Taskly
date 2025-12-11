using System.Net;
using Api.Features.Tags.Model;
using Api.Features.Todos.Model;
using FluentAssertions;
using IntegrationTests.Extensions;

namespace IntegrationTests.Tests.Tags;

public sealed class RemoveTagTests(TestingFixture fixture) : TestingBase(fixture)
{
    private static Todo CreateTodo(
        UserId userId)
    {
        return Todo.Create(
            "Test Todo",
            "Test Description",
            TodoPriority.Medium,
            userId
        );
    }

    private static Tag CreateTag(
        string name,
        UserId userId)
    {
        return new Tag(name, userId);
    }

    [Fact]
    public async Task DeleteTag_Should_Return401_When_Unauthenticated()
    {
        // Arrange
        var unauthenticated = CreateUnauthenticatedClient();
        var randomId = TagId.From(Guid.NewGuid());

        // Act
        var response = await unauthenticated.DeleteTagAsync(
            randomId,
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteTag_Should_Return404_When_TagDoesNotExist()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        var randomId = TagId.From(Guid.NewGuid());

        // Act
        var response = await client.DeleteTagAsync(
            randomId,
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        await response.ContainsErrorCode("Tag.NotFound", CurrentCancellationToken);
    }

    [Fact]
    public async Task DeleteTag_Should_RemoveTag_When_TagExists()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        var userId = CurrentUserId;

        var tag = CreateTag("TagToDelete", userId);

        await using var dbContext = GetDbContext();
        dbContext.Tags.Add(tag);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        // Act
        var response = await client.DeleteTagAsync(
            tag.Id,
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        (await GetDbContext().Tags
                .AnyAsync(t => t.Id == tag.Id, CurrentCancellationToken))
            .Should().BeFalse();
    }

    [Fact]
    public async Task DeleteTag_Should_RemoveTagFromTodosBeforeDeletion()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        var userId = CurrentUserId;

        var todo = CreateTodo(userId);
        var tag = CreateTag("TagInUse", userId);

        todo.Tags.Add(tag);

        await using var dbContext = GetDbContext();
        dbContext.Todos.Add(todo);
        dbContext.Tags.Add(tag);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        // Act
        var response = await client.DeleteTagAsync(
            tag.Id,
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedTodo = await GetDbContext().Todos
            .Include(t => t.Tags)
            .AsNoTracking()
            .FirstAsync(t => t.Id == todo.Id, CurrentCancellationToken);

        updatedTodo.Tags.Should().BeEmpty();

        (await GetDbContext().Tags.AnyAsync(t => t.Id == tag.Id, CurrentCancellationToken))
            .Should().BeFalse();
    }
}