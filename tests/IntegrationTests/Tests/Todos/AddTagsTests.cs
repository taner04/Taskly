using System.Net;
using Api.Features.Tags.Model;
using Api.Features.Todos.Endpoints;
using Api.Features.Todos.Model;
using FluentAssertions;

namespace IntegrationTests.Tests.Todos;

public sealed class AddTagsTests(TestingFixture fixture) : TestingBase(fixture)
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
        var client = CreateAuthenticatedUserClient();
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
        var client = CreateAuthenticatedUserClient();
        var userId = CurrentUserId;
        var todo = CreateTodo(userId);

        await using var dbContext = GetDbContext();
        dbContext.Add(todo);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

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
        var client = CreateAuthenticatedUserClient();
        var userId = CurrentUserId;

        var todo = CreateTodo(userId);

        await using var dbContext = GetDbContext();
        dbContext.Add(todo);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        var tag1 = CreateTag("TagOne", userId);
        var tag2 = CreateTag("TagTwo", userId);
        dbContext.AddRange(tag1, tag2);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

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

        var updated = await GetDbContext().Todos
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
        var client = CreateAuthenticatedUserClient();
        var userId = CurrentUserId;

        var tagA = CreateTag("Alpha", userId);
        var tagB = CreateTag("Beta", userId);

        await using var dbContext = GetDbContext();
        dbContext.Add(tagA);
        dbContext.Add(tagB);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        var todo = CreateTodo(userId);

        dbContext.Add(todo);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

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

        var updated = await GetDbContext().Todos
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
        var client = CreateAuthenticatedUserClient();
        var userId = CurrentUserId;

        var todo = CreateTodo(userId);
        var tagValid = CreateTag("Valid", userId);
        var tagForeign = CreateTag("Foreign", UserId.EmptyId);

        await using var dbContext = GetDbContext();
        dbContext.Add(todo);
        dbContext.Add(tagValid);
        dbContext.Add(tagForeign);

        await dbContext.SaveChangesAsync(CurrentCancellationToken);

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

        var updated = await GetDbContext().Todos
            .Include(t => t.Tags)
            .AsNoTracking()
            .FirstAsync(t => t.Id == todo.Id, CurrentCancellationToken);

        updated.Tags.Should().HaveCount(1);
        updated.Tags.First().Id.Should().Be(tagValid.Id);
    }
}