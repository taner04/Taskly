using System.Net;
using System.Net.Http.Json;
using Api.Features.Shared;
using Api.Features.Todos.Domain;
using Api.Infrastructure.Data;
using IntegrationTests.Common;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.Tests.Todos;

public class GetTodosTest(TestingFixture fixture) : TestingBase(fixture)
{
    [Fact]
    public async Task CompleteTodo_WhenTodoExistsAndBelongsToUser_ReturnsSuccessAndUpdatesTodo()
    {
        var client = CreateClient();
        var userId = client.GetUserId();

        SetAuthenticatedUser(userId);
        
        var todo = Todo.TryCreate("Test", "Description", TodoPriority.Medium, userId).Value;
        DbContext.Todos.Add(todo);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var url = ApiRoutes.Todos.Complete.WithId(todo.Id.Value);

        var response = await client.PostAsJsonAsync(url, new { completed = true }, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var updated = await DbContext.Todos.SingleAsync(t => t.Id == todo.Id, CurrentCancellationToken);
        Assert.True(updated.IsCompleted);
    }

    [Fact]
    public async Task CompleteTodo_WhenTodoDoesNotExist_ReturnsNotFound()
    {
        var client = CreateClient();
        var url = ApiRoutes.Todos.Complete.WithId(Guid.NewGuid());

        var response = await client.PostAsJsonAsync(url, new { completed = true }, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CompleteTodo_WhenTodoBelongsToAnotherUser_ReturnsNotFound()
    {
        var client = CreateClient();
        var userId = client.GetUserId();

        var todo = Todo.TryCreate("Test", "Desc", TodoPriority.Medium, "auth0|otherUser").Value;
        DbContext.Todos.Add(todo);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var url = ApiRoutes.Todos.Complete.WithId(todo.Id.Value);

        var response = await client.PostAsJsonAsync(url, new { completed = true }, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}