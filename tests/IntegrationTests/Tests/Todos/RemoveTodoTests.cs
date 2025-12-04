using System.Net;
using Api.Features.Todos.Model;
using Api.Shared.Api;
using IntegrationTests.Extensions;

namespace IntegrationTests.Tests.Todos;

public class RemoveTodoTests(TestingFixture fixture) : TestingBase(fixture)
{
    private static string GetRoute(
        Guid todoId)
    {
        return Routes.Todos.Remove.Replace("{todoId}", todoId.ToString());
    }

    [Fact]
    public async Task RemoveTodo_WhenExists_RemovesTodo()
    {
        var client = CreateAuthenticatedClient();
        var userId = client.GetUserId();

        var todo = new Todo("Task1", "Description", TodoPriority.Medium, userId);

        DbContext.Todos.Add(todo);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var url = GetRoute(todo.Id.Value);

        var response = await client.DeleteAsync(url, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var exists = await DbContext.Todos
            .AnyAsync(t => t.Id == todo.Id, CurrentCancellationToken);

        Assert.False(exists);
    }

    [Fact]
    public async Task RemoveTodo_WhenTodoNotFound_ReturnsNotFound()
    {
        var client = CreateAuthenticatedClient();

        var url = GetRoute(Guid.NewGuid());

        var response = await client.DeleteAsync(url, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task RemoveTodo_WhenTodoBelongsToAnotherUser_ReturnsNotFound()
    {
        var client = CreateAuthenticatedClient();

        var todo = new Todo("OtherTask", "Desc", TodoPriority.High, "auth0|other");

        DbContext.Todos.Add(todo);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var url = GetRoute(todo.Id.Value);

        var response = await client.DeleteAsync(url, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task RemoveTodo_WhenUserNotAuthenticated_ReturnsUnauthorized()
    {
        var client = CreateUnauthenticatedClient();

        var url = GetRoute(Guid.NewGuid());

        var response = await client.DeleteAsync(url, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}