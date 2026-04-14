using Taskly.WebApi.Common.Shared.Pagination;
using Taskly.WebApi.IntegrationTests.Infrastructure;
using Taskly.WebApi.IntegrationTests.Infrastructure.Fixtures;

namespace Taskly.WebApi.IntegrationTests.Tests.Todos;

public sealed class GetTodosTests(TestingFixture fixture) : TestingBase(fixture)
{
    private static Todo CreateTodo(
        UserId userId,
        string title = "Test Todo",
        string description = "Test Description",
        TodoPriority priority = TodoPriority.Medium,
        bool completed = false)
    {
        var todo = Todo.Create(title, description, priority, userId);
        todo.IsCompleted = completed;

        return todo;
    }

    private static Tag CreateTag(string name, UserId userId) => Tag.Create(name, userId);

    private static Attachment CreateAttachment(
        Todo todo) =>
        Attachment.Create(
            todo.Id,
            "file.txt",
            "text/plain"
        );

    [Fact]
    public async Task GetTodos_Should_Return401_When_Unauthenticated()
    {
        // Arrange
        var client = GetUnauthenticatedClient();

        // Act
        var response = await client.GetTodosAsync(
            new GetTodos.Query(0, 10),
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetTodos_Should_ReturnEmptyList_When_UserHasNoTodos()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();

        // Act
        var response = await client.GetTodosAsync(
            new GetTodos.Query(0, 10),
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.MapTo<PaginationResult<GetTodos.Response>>(CurrentCancellationToken);
        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task GetTodos_Should_ReturnOnlyTodos_ForAuthenticatedUser()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        var userId = CurrentUserId;

        var todo1 = CreateTodo(userId, "Todo A");
        var todo2 = CreateTodo(userId, "Todo B");

        var foreignUserId = await CreateForeignUserAsync();
        var foreignTodo = CreateTodo(foreignUserId, "Foreign Todo");

        await using var dbContext = GetDbContext();
        dbContext.Add(todo1);
        dbContext.Add(todo2);
        dbContext.Add(foreignTodo);

        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        // Act
        var response = await client.GetTodosAsync(
            new GetTodos.Query(0, 10),
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.MapTo<PaginationResult<GetTodos.Response>>(CurrentCancellationToken);

        result.Items.Should().HaveCount(2);
        result.Items.Select(t => t.Title).Should().BeEquivalentTo("Todo A", "Todo B");
        result.Items.Should().OnlyContain(t => t.UserId == userId);
    }

    [Fact]
    public async Task GetTodos_Should_ReturnTodos_WithTags_And_Attachments()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        var userId = CurrentUserId;

        var todo = CreateTodo(userId);

        var tag1 = CreateTag("Tag1", userId);
        var tag2 = CreateTag("Tag2", userId);

        var attachment = CreateAttachment(todo);

        todo.Tags.Add(tag1);
        todo.Tags.Add(tag2);
        todo.Attachments.Add(attachment);

        await using var dbContext = GetDbContext();
        dbContext.Add(tag1);
        dbContext.Add(tag2);
        dbContext.Add(todo);
        dbContext.Add(attachment);

        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        // Act
        var response = await client.GetTodosAsync(
            new GetTodos.Query(0, 10),
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.MapTo<PaginationResult<GetTodos.Response>>(CurrentCancellationToken);

        result.Items.Should().HaveCount(1);

        var item = result.Items.First();

        item.Id.Should().Be(item.Id);
        item.Title.Should().Be(todo.Title);
        item.Description.Should().Be(todo.Description);
        item.Priority.Should().Be(todo.Priority);
        item.IsCompleted.Should().Be(todo.IsCompleted);

        item.Tags.Should().HaveCount(2);
        item.Tags.Select(t => t.Id).Should().BeEquivalentTo(new[] { tag1.Id.Value, tag2.Id.Value });

        item.Attachments.Should().HaveCount(1);
        item.Attachments.First().Id.Should().Be(attachment.Id.Value);
    }
}