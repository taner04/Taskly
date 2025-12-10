using System.Net;
using Api.Features.Attachments.Models;
using Api.Features.Tags.Model;
using Api.Features.Todos.Endpoints;
using Api.Features.Todos.Model;
using FluentAssertions;
using IntegrationTests.Extensions;

namespace IntegrationTests.Tests.Todos;

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
        todo.SetCompletionStatus(completed);
        return todo;
    }

    private static Tag CreateTag(string name, UserId userId)
    {
        return new Tag(name, userId);
    }

    private static Attachment CreateAttachment(Todo todo)
    {
        return Attachment.CreatePending(
            todo.Id,
            "file.txt",
            "text/plain"
        );
    }

    [Fact]
    public async Task GetTodos_Should_Return401_When_Unauthenticated()
    {
        // Arrange
        var client = CreateUnauthenticatedClient();

        // Act
        var response = await client.GetTodosAsync(CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetTodos_Should_ReturnEmptyList_When_UserHasNoTodos()
    {
        // Arrange
        var client = CreateAuthenticatedClient();

        // Act
        var response = await client.GetTodosAsync(CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var list = await response.MapTo<List<GetTodos.Response>>(CurrentCancellationToken);
        list.Should().BeEmpty();
    }

    [Fact]
    public async Task GetTodos_Should_ReturnOnlyTodos_ForAuthenticatedUser()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var userId = GetCurrentUserId();

        var todo1 = CreateTodo(userId, "Todo A");
        var todo2 = CreateTodo(userId, "Todo B");

        var foreignTodo = CreateTodo(UserId.EmptyId, "Foreign Todo");

        DbContext.Add(todo1);
        DbContext.Add(todo2);
        DbContext.Add(foreignTodo);

        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        // Act
        var response = await client.GetTodosAsync(CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var list = await response.MapTo<List<GetTodos.Response>>(CurrentCancellationToken);

        list.Should().HaveCount(2);
        list.Select(t => t.Title).Should().BeEquivalentTo("Todo A", "Todo B");
        list.Should().OnlyContain(t => t.UserId == userId);
    }

    [Fact]
    public async Task GetTodos_Should_ReturnTodos_WithTags_And_Attachments()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var userId = GetCurrentUserId();

        var todo = CreateTodo(userId);

        var tag1 = CreateTag("Tag1", userId);
        var tag2 = CreateTag("Tag2", userId);

        var attachment = CreateAttachment(todo);

        todo.Tags.Add(tag1);
        todo.Tags.Add(tag2);
        todo.Attachments.Add(attachment);

        DbContext.Add(tag1);
        DbContext.Add(tag2);
        DbContext.Add(todo);
        DbContext.Add(attachment);

        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        // Act
        var response = await client.GetTodosAsync(CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var list = await response.MapTo<List<GetTodos.Response>>(CurrentCancellationToken);

        list.Should().HaveCount(1);

        var item = list.First();

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