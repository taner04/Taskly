using System.Net;
using Api.Features.Todos.Model;
using IntegrationTests.Extensions;

namespace IntegrationTests.Tests.Todos;

public class DeleteTodoTests(TestingFixture fixture) : TestingBase(fixture)
{
    [Fact]
    public async Task DeleteTodo_WhenTodoExistsAndBelongsToUser_ReturnsSuccessAndDeletesTodo()
    {
        var client = CreateAuthenticatedClient();
        var userId = client.GetUserId();

        // Seed a todo belonging to the authenticated user
        var todo = Todo.TryCreate("Test", "Description", TodoPriority.Medium, userId).Value;
        DbContext.Todos.Add(todo);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var url = ApiRoutes.Todos.Delete.ParseTodoRoute(todo.Id.Value);

        var response = await client.DeleteAsync(url, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Ensure the Todo was removed
        var exists = await DbContext.Todos
            .AsNoTracking()
            .AnyAsync(t => t.Id == todo.Id, CurrentCancellationToken);

        Assert.False(exists);
    }

    [Fact]
    public async Task DeleteTodo_WhenTodoDoesNotExist_ReturnsNotFound()
    {
        var client = CreateAuthenticatedClient();

        var url = ApiRoutes.Todos.Delete.ParseTodoRoute(Guid.NewGuid());

        var response = await client.DeleteAsync(url, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteTodo_WhenTodoBelongsToAnotherUser_ReturnsNotFound()
    {
        var client = CreateAuthenticatedClient();

        // Seed a todo that is NOT owned by the authenticated user
        var todo = Todo.TryCreate("Test", "Description", TodoPriority.Low, "auth0|someOtherUser").Value;
        DbContext.Todos.Add(todo);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var url = ApiRoutes.Todos.Delete.ParseTodoRoute(todo.Id.Value);

        var response = await client.DeleteAsync(url, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteTodo_WhenUserIsNotAuthenticated_ReturnsUnauthorized()
    {
        var client = CreateUnauthenticatedClient();

        var url = ApiRoutes.Todos.Delete.ParseTodoRoute(Guid.NewGuid());

        var response = await client.DeleteAsync(url, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}