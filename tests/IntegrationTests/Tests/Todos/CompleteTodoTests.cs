using System.Net;
using System.Net.Http.Json;
using Api.Features.Todos.Endpoints;
using Api.Features.Todos.Model;
using Api.Shared.Api;
using IntegrationTests.Extensions;

namespace IntegrationTests.Tests.Todos;

public class CompleteTodoTests(TestingFixture fixture) : TestingBase(fixture)
{
    private static string GetRoute(
        Guid todoId)
    {
        return Routes.Todos.Complete.Replace("{todoId:guid}", todoId.ToString());
    }

    [Fact]
    public async Task CompleteTodo_WhenTodoExistsAndBelongsToUser_UpdatesCompletionStatus()
    {
        var client = CreateAuthenticatedClient();
        var userId = client.GetUserId();

        var todo = new Todo("Task1", "Description", TodoPriority.Medium, userId);

        DbContext.Todos.Add(todo);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var body = new CompleteTodo.Command.CommandBody
        {
            Completed = true
        };

        var response = await client.PostAsJsonAsync(GetRoute(todo.Id.Value), body, CurrentCancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var updated = await DbContext.Todos
            .AsNoTracking()
            .SingleAsync(t => t.Id == todo.Id, CurrentCancellationToken);

        Assert.True(updated.IsCompleted);
    }

    [Fact]
    public async Task CompleteTodo_WhenTodoDoesNotExist_ReturnsNotFound()
    {
        var client = CreateAuthenticatedClient();

        var body = new CompleteTodo.Command.CommandBody
        {
            Completed = true
        };

        var response = await client.PostAsJsonAsync(GetRoute(Guid.NewGuid()), body, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problem = await response.ReadProblemAsync();
        Assert.Equal("Todo.NotFound", problem.ErrorCode);
    }

    [Fact]
    public async Task CompleteTodo_WhenBelongsToAnotherUser_ReturnsNotFound()
    {
        var client = CreateAuthenticatedClient();

        var todo = new Todo("Task1", "Desc", TodoPriority.Medium, "auth0|other");

        DbContext.Todos.Add(todo);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var body = new CompleteTodo.Command.CommandBody
        {
            Completed = true
        };

        var response = await client.PostAsJsonAsync(GetRoute(todo.Id.Value), body, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problem = await response.ReadProblemAsync();
        Assert.Equal("Todo.NotFound", problem.ErrorCode);
    }

    [Fact]
    public async Task CompleteTodo_WhenUserNotAuthenticated_ReturnsUnauthorized()
    {
        var client = CreateUnauthenticatedClient();

        var body = new CompleteTodo.Command.CommandBody
        {
            Completed = true
        };

        var response = await client.PostAsJsonAsync(GetRoute(Guid.NewGuid()), body, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CompleteTodo_WhenSettingSameValue_DoesNotChangeOtherFields()
    {
        var client = CreateAuthenticatedClient();
        var userId = client.GetUserId();

        var todo = new Todo("TaskX", "Desc", TodoPriority.Low, userId);
        todo.SetCompletionStatus(false);

        DbContext.Todos.Add(todo);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var body = new CompleteTodo.Command.CommandBody
        {
            Completed = false
        };

        var response = await client.PostAsJsonAsync(GetRoute(todo.Id.Value), body, CurrentCancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var updated = await DbContext.Todos
            .AsNoTracking()
            .SingleAsync(t => t.Id == todo.Id, CurrentCancellationToken);

        Assert.False(updated.IsCompleted);
        Assert.Equal(todo.Title, updated.Title);
        Assert.Equal(todo.Priority, updated.Priority);
    }
}