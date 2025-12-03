using System.Net;
using System.Net.Http.Json;
using Api.Features.Todos.Model;
using Api.Shared.Features.Api;
using IntegrationTests.Extensions;

namespace IntegrationTests.Tests.Todos;

public class GetTodosTests(TestingFixture fixture) : TestingBase(fixture)
{
    [Fact]
    public async Task GetTodos_WhenUserHasTodos_ReturnsOnlyUserTodos()
    {
        var client = CreateAuthenticatedClient();
        var userId = client.GetUserId();

        // Seed 2 todos belonging to authenticated user
        var todo1 = Todo.TryCreate("User Todo 1", "Description 1", TodoPriority.Medium, userId).Value;
        var todo2 = Todo.TryCreate("User Todo 2", "Description 2", TodoPriority.Low, userId).Value;

        // Seed a todo belonging to another user
        var otherTodo = Todo.TryCreate("Other User Todo", "Description", TodoPriority.High, "auth0|otherUser").Value;

        DbContext.Todos.AddRange(todo1, todo2, otherTodo);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var response = await client.GetAsync(Routes.Todos.GetTodos, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<List<GetTodos.TodoDto>>(CurrentCancellationToken);
        Assert.NotNull(result);

        Assert.Equal(2, result.Count);
        Assert.All(result, t => Assert.Equal(userId, t.UserId));
    }

    [Fact]
    public async Task GetTodos_WhenUserHasNoTodos_ReturnsEmptyList()
    {
        var client = CreateAuthenticatedClient();

        var response = await client.GetAsync(Routes.Todos.GetTodos, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<List<GetTodos.TodoDto>>(CurrentCancellationToken);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetTodos_WhenTodosBelongToOtherUsers_ReturnsEmptyList()
    {
        var client = CreateAuthenticatedClient();

        // Seed only other-user todos
        var otherTodo1 = Todo.TryCreate("Other 1", "Desc", TodoPriority.Low, "auth0|otherUser").Value;
        var otherTodo2 = Todo.TryCreate("Other 2", "Desc", TodoPriority.High, "auth0|someoneElse").Value;

        DbContext.Todos.AddRange(otherTodo1, otherTodo2);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var response = await client.GetAsync(Routes.Todos.GetTodos, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<List<GetTodos.TodoDto>>(CurrentCancellationToken);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetTodos_WhenUserIsNotAuthenticated_ReturnsUnauthorized()
    {
        var client = CreateUnauthenticatedClient();

        var response = await client.GetAsync(Routes.Todos.GetTodos, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}