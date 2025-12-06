using System.Net;
using Api.Features.Tags.Model;
using Api.Features.Todos.Endpoints;
using Api.Features.Todos.Model;
using FluentAssertions;
using IntegrationTests.Extensions;

namespace IntegrationTests.Tests.Todos;

public sealed class AddTagsTests(TestingFixture fixture) : TestingBase(fixture)
{
    private static Todo CreateTodo(string userId)
    {
        return new Todo(
            "Test Todo",
            "Test Description",
            TodoPriority.Medium,
            userId
        );
    }

    private static Tag CreateTag(string name, string userId)
    {
        return new Tag(name, userId);
    }

    [Fact]
    public async Task AddTags_Should_Return401_When_Unauthenticated()
    {
        // Arrange
        var unauthenticated = CreateUnauthenticatedClient();
        var todoId = TodoId.From(Guid.NewGuid());

        // Act
        var response = await unauthenticated.AddTagsToTodoAsync(
            todoId,
            new AddTags.Command.CommandBody
            {
                TagIds = [TagId.From(Guid.NewGuid())]
            },
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AddTags_Should_Return404_When_TodoDoesNotExist()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var todoId = TodoId.From(Guid.NewGuid());

        // Act
        var response = await client.AddTagsToTodoAsync(
            todoId,
            new AddTags.Command.CommandBody
            {
                TagIds = [TagId.From(Guid.NewGuid())]
            },
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        await response.ContainsErrorCode("Todo.NotFound", CurrentCancellationToken);
    }

    [Fact]
    public async Task AddTags_Should_Return404_When_TagsDoNotExist()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var userId = GetCurrentUserId();
        var todo = CreateTodo(userId);

        DbContext.Add(todo);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        // Act
        var response = await client.AddTagsToTodoAsync(
            todo.Id,
            new AddTags.Command.CommandBody
            {
                TagIds = [TagId.From(Guid.NewGuid())]
            },
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        await response.ContainsErrorCode("Tag.NotFound", CurrentCancellationToken);
    }

    [Fact]
    public async Task AddTags_Should_AddTagsToTodo()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var userId = GetCurrentUserId();

        var todo = CreateTodo(userId);
        DbContext.Add(todo);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var tag1 = CreateTag("TagOne", userId);
        var tag2 = CreateTag("TagTwo", userId);
        DbContext.AddRange(tag1, tag2);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        // Act
        var response = await client.AddTagsToTodoAsync(
            todo.Id,
            new AddTags.Command.CommandBody
            {
                TagIds = [tag1.Id, tag2.Id]
            },
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updated = await DbContext.Todos
            .Include(t => t.Tags)
            .AsNoTracking()
            .FirstAsync(t => t.Id == todo.Id, CurrentCancellationToken);

        updated.Tags.Should().HaveCount(2);
        updated.Tags.Select(t => t.Id).Should().BeEquivalentTo([tag1.Id, tag2.Id]);
    }

    [Fact]
    public async Task AddTags_Should_NotDuplicateExistingTags()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var userId = GetCurrentUserId();

        var tagA = CreateTag("Alpha", userId);
        var tagB = CreateTag("Beta", userId);

        DbContext.Add(tagA);
        DbContext.Add(tagB);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var todo = CreateTodo(userId);

        DbContext.Add(todo);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        // Act
        var response = await client.AddTagsToTodoAsync(
            todo.Id,
            new AddTags.Command.CommandBody
            {
                TagIds = [tagA.Id, tagB.Id]
            },
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updated = await DbContext.Todos
            .Include(t => t.Tags)
            .AsNoTracking()
            .FirstAsync(t => t.Id == todo.Id, CurrentCancellationToken);

        updated.Tags.Should().HaveCount(2);
        updated.Tags.Select(t => t.Id).Should().BeEquivalentTo([tagA.Id, tagB.Id]);
    }


    [Fact]
    public async Task AddTags_Should_NotAddTagsFromAnotherUser()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var userId = GetCurrentUserId();

        var todo = CreateTodo(userId);
        var tagValid = CreateTag("Valid", userId);
        var tagForeign = CreateTag("Foreign", "other-user");

        DbContext.Add(todo);
        DbContext.Add(tagValid);
        DbContext.Add(tagForeign);

        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        // Act
        var response = await client.AddTagsToTodoAsync(
            todo.Id,
            new AddTags.Command.CommandBody
            {
                TagIds = [tagValid.Id, tagForeign.Id]
            },
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updated = await DbContext.Todos
            .Include(t => t.Tags)
            .AsNoTracking()
            .FirstAsync(t => t.Id == todo.Id, CurrentCancellationToken);

        updated.Tags.Should().HaveCount(1);
        updated.Tags.First().Id.Should().Be(tagValid.Id);
    }
}