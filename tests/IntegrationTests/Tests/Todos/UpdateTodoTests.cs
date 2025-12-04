using System.Net;
using System.Net.Http.Json;
using Api.Features.Todos.Endpoints;
using Api.Features.Todos.Model;
using Api.Shared.Api;
using IntegrationTests.Extensions;

namespace IntegrationTests.Tests.Todos;

public class UpdateTodoTests(TestingFixture fixture) : TestingBase(fixture)
{
    private static string GetRoute(
        Guid todoId)
    {
        return Routes.Todos.Update.Replace("{todoId}", todoId.ToString());
    }

    [Fact]
    public async Task UpdateTodo_WhenTodoExists_UpdatesTodo()
    {
        var client = CreateAuthenticatedClient();
        var userId = client.GetUserId();

        var todo = new Todo("Old Title", "Old Desc", TodoPriority.Low, userId);

        DbContext.Todos.Add(todo);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var body = new UpdateTodo.Command.CommandBody
        {
            Title = "New Title",
            Description = "New Description",
            Priority = TodoPriority.High
        };

        var url = GetRoute(todo.Id.Value);

        var response = await client.PutAsJsonAsync(url, body, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var updated = await DbContext.Todos.AsNoTracking()
            .SingleAsync(t => t.Id == todo.Id, CurrentCancellationToken);

        Assert.Equal("New Title", updated.Title);
        Assert.Equal("New Description", updated.Description);
        Assert.Equal(TodoPriority.High, updated.Priority);
    }

    [Fact]
    public async Task UpdateTodo_WhenTodoNotFound_ReturnsNotFound()
    {
        var client = CreateAuthenticatedClient();

        var body = new UpdateTodo.Command.CommandBody
        {
            Title = "X",
            Description = "Y",
            Priority = TodoPriority.Low
        };

        var url = GetRoute(Guid.NewGuid());

        var response = await client.PutAsJsonAsync(url, body, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateTodo_WhenTodoBelongsToOtherUser_ReturnsNotFound()
    {
        var client = CreateAuthenticatedClient();

        var todo = new Todo("OtherUser", "Desc", TodoPriority.Medium, "auth0|other");

        DbContext.Todos.Add(todo);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var body = new UpdateTodo.Command.CommandBody
        {
            Title = "Updated",
            Description = "Updated Desc",
            Priority = TodoPriority.Low
        };

        var url = GetRoute(todo.Id.Value);

        var response = await client.PutAsJsonAsync(url, body, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateTodo_WhenUserNotAuthenticated_ReturnsUnauthorized()
    {
        var client = CreateUnauthenticatedClient();

        var body = new UpdateTodo.Command.CommandBody
        {
            Title = "Title",
            Description = "Desc",
            Priority = TodoPriority.Low
        };

        var url = GetRoute(Guid.NewGuid());

        var response = await client.PutAsJsonAsync(url, body, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}