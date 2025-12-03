using System.Net;
using System.Net.Http.Json;
using Api.Features.Shared.Api;
using Api.Features.Todos.Model;
using IntegrationTests.Extensions;

namespace IntegrationTests.Tests.Todos;

public class CompleteTodoTests(TestingFixture fixture) : TestingBase(fixture)
{
    [Fact]
    public async Task CompleteTodo_WhenTodoExistsAndBelongsToUser_ReturnsSuccessAndUpdatesTodo()
    {
        var client = CreateAuthenticatedClient();
        var userId = client.GetUserId();

        var todo = Todo.TryCreate("Test", "Description", TodoPriority.Medium, userId).Value;
        DbContext.Todos.Add(todo);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var url = Routes.Todos.Complete.ParseTodoRoute(todo.Id.Value);
        var postBodyObject = new CompleteTodo.Command.CommandBody
        {
            Completed = true
        };

        var response = await client.PostAsJsonAsync(url, postBodyObject, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var updated = await DbContext.Todos.AsNoTracking().SingleAsync(t => t.Id == todo.Id, CurrentCancellationToken);
        Assert.True(updated.IsCompleted);
    }

    [Fact]
    public async Task CompleteTodo_WhenTodoDoesNotExist_ReturnsNotFound()
    {
        var client = CreateAuthenticatedClient();

        var url = Routes.Todos.Complete.ParseTodoRoute(Guid.NewGuid());
        var postBodyObject = new CompleteTodo.Command.CommandBody
        {
            Completed = true
        };

        var response = await client.PostAsJsonAsync(url, postBodyObject, CurrentCancellationToken);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CompleteTodo_WhenTodoBelongsToAnotherUser_ReturnsNotFound()
    {
        var client = CreateAuthenticatedClient();

        var todo = Todo.TryCreate("Test", "Desc", TodoPriority.Medium, "auth0|otherUser").Value;
        DbContext.Todos.Add(todo);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var url = Routes.Todos.Complete.ParseTodoRoute(todo.Id.Value);
        var postBodyObject = new CompleteTodo.Command.CommandBody
        {
            Completed = true
        };

        var response = await client.PostAsJsonAsync(url, postBodyObject, CurrentCancellationToken);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CompleteTodo_WhenSettingCompletedToFalse_UpdatesTodoStatus()
    {
        var client = CreateAuthenticatedClient();
        var userId = client.GetUserId();

        var todo = Todo.TryCreate("Test", "Description", TodoPriority.Medium, userId).Value;
        // Manually set it as completed first
        todo.SetCompletionStatus(true);
        DbContext.Todos.Add(todo);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var url = Routes.Todos.Complete.ParseTodoRoute(todo.Id.Value);
        var postBodyObject = new CompleteTodo.Command.CommandBody
        {
            Completed = false
        };

        var response = await client.PostAsJsonAsync(url, postBodyObject, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var updated = await DbContext.Todos.AsNoTracking().SingleAsync(t => t.Id == todo.Id, CurrentCancellationToken);
        Assert.False(updated.IsCompleted);
    }

    [Fact]
    public async Task CompleteTodo_WhenUserIsNotAuthenticated_ReturnsUnauthorized()
    {
        var client = CreateUnauthenticatedClient();

        var url = Routes.Todos.Complete.ParseTodoRoute(Guid.NewGuid());
        var postBodyObject = new CompleteTodo.Command.CommandBody
        {
            Completed = true
        };

        var response = await client.PostAsJsonAsync(url, postBodyObject, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}