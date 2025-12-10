using System.Net;
using Api.Features.Tags.Model;
using Api.Features.Todos.Model;
using FluentAssertions;
using IntegrationTests.Extensions;

namespace IntegrationTests.Tests.Tags;

public sealed class DeleteTagTests(TestingFixture fixture) : TestingBase(fixture)
{
    private static Todo CreateTodo(UserId userId)
    {
        return new Todo(
            "Test Todo",
            "Test Description",
            TodoPriority.Medium,
            userId
        );
    }

    private static Tag CreateTag(string name, UserId userId)
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
        var client = CreateAuthenticatedClient();
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
        var client = CreateAuthenticatedClient();
        var userId = GetCurrentUserId();

        var tag = CreateTag("TagToDelete", userId);

        DbContext.Tags.Add(tag);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        // Act
        var response = await client.DeleteTagAsync(
            tag.Id,
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        (await DbContext.Tags
                .AnyAsync(t => t.Id == tag.Id, CurrentCancellationToken))
            .Should().BeFalse();
    }

    [Fact]
    public async Task DeleteTag_Should_RemoveTagFromTodosBeforeDeletion()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var userId = GetCurrentUserId();

        var todo = CreateTodo(userId);
        var tag = CreateTag("TagInUse", userId);

        todo.Tags.Add(tag);

        DbContext.Todos.Add(todo);
        DbContext.Tags.Add(tag);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        // Act
        var response = await client.DeleteTagAsync(
            tag.Id,
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedTodo = await DbContext.Todos
            .Include(t => t.Tags)
            .AsNoTracking()
            .FirstAsync(t => t.Id == todo.Id, CurrentCancellationToken);

        updatedTodo.Tags.Should().BeEmpty();

        (await DbContext.Tags.AnyAsync(t => t.Id == tag.Id, CurrentCancellationToken))
            .Should().BeFalse();
    }
}