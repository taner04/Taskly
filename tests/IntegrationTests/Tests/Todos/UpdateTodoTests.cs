using System.Net;
using System.Net.Http.Json;
using IntegrationTests.Extensions;

namespace IntegrationTests.Tests.Todos;

public class UpdateTodoTests(TestingFixture fixture) : TestingBase(fixture)
{
    [Fact]
    public async Task UpdateTodo_WhenTodoExistsAndBelongsToUser_UpdatesTodoAndReturnsSuccess()
    {
        var client = CreateAuthenticatedClient();
        var userId = client.GetUserId();

        var todo = Todo.TryCreate("Original", "Original Description", TodoPriority.Medium, userId).Value;
        DbContext.Todos.Add(todo);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var url = ApiRoutes.Todos.Update.WithId(todo.Id.Value);
        var body = new
        {
            Title = "Updated Title",
            Description = "Updated Description",
            Priority = TodoPriority.High
        };

        var response = await client.PutAsJsonAsync(url, body, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var updated = await DbContext.Todos
            .AsNoTracking()
            .SingleAsync(t => t.Id == todo.Id, CurrentCancellationToken);

        Assert.Equal(body.Title, updated.Title);
        Assert.Equal(body.Description, updated.Description);
        Assert.Equal(body.Priority, updated.Priority);
    }

    [Fact]
    public async Task UpdateTodo_WhenTodoDoesNotExist_ReturnsNotFound()
    {
        var client = CreateAuthenticatedClient();

        var url = ApiRoutes.Todos.Update.WithId(Guid.NewGuid());

        var body = new
        {
            Title = "Does Not Matter",
            Description = "Still Does Not Matter",
            Priority = TodoPriority.Low
        };

        var response = await client.PutAsJsonAsync(url, body, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateTodo_WhenTodoBelongsToAnotherUser_ReturnsNotFound()
    {
        var client = CreateAuthenticatedClient();

        var todo = Todo.TryCreate("Other User Todo", "Desc", TodoPriority.Medium, "auth0|otherUser").Value;
        DbContext.Todos.Add(todo);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var url = ApiRoutes.Todos.Update.WithId(todo.Id.Value);

        var body = new
        {
            Title = "Updated Title",
            Description = "Updated Description",
            Priority = TodoPriority.High
        };

        var response = await client.PutAsJsonAsync(url, body, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateTodo_WhenTitleIsTooShort_ReturnsBadRequest()
    {
        var client = CreateAuthenticatedClient();
        var userId = client.GetUserId();

        var todo = Todo.TryCreate("Valid Title", "Valid Description", TodoPriority.Medium, userId).Value;
        DbContext.Todos.Add(todo);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var url = ApiRoutes.Todos.Update.WithId(todo.Id.Value);

        var body = new
        {
            Title = "aa", // invalid (min length is 3)
            Description = "Still Valid Description",
            Priority = TodoPriority.Low
        };

        var response = await client.PutAsJsonAsync(url, body, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateTodo_WhenUserIsNotAuthenticated_ReturnsUnauthorized()
    {
        var client = CreateUnauthenticatedClient();

        var url = ApiRoutes.Todos.Update.WithId(Guid.NewGuid());

        var body = new
        {
            Title = "Some Title",
            Description = "Some Description",
            Priority = TodoPriority.Medium
        };

        var response = await client.PutAsJsonAsync(url, body, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
