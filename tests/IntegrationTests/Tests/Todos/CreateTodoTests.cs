using System.Net;
using System.Net.Http.Json;
using Api.Features.Shared.Api;
using Api.Features.Todos.Model;
using IntegrationTests.Extensions;

namespace IntegrationTests.Tests.Todos;

public class CreateTodoTests(TestingFixture fixture) : TestingBase(fixture)
{
    [Fact]
    public async Task CreateTodo_WhenDataIsValid_ReturnsSuccessAndPersistsTodo()
    {
        var client = CreateAuthenticatedClient();
        var userId = client.GetUserId();

        var command = new CreateTodo.Command
        {
            Title = "Valid Title",
            Description = "Valid Description",
            Priority = TodoPriority.Low
        };

        var response = await client.PostAsJsonAsync(Routes.Todos.Create, command, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Ensure the todo was persisted
        var created = await DbContext.Todos
            .AsNoTracking()
            .SingleAsync(t => t.Title == command.Title && t.UserId == userId, CurrentCancellationToken);

        Assert.Equal(command.Title, created.Title);
        Assert.Equal(command.Description, created.Description);
        Assert.Equal(command.Priority, created.Priority);
        Assert.False(created.IsCompleted);
    }

    [Fact]
    public async Task CreateTodo_WhenTitleIsTooShort_ReturnsValidationError()
    {
        var client = CreateAuthenticatedClient();

        var command = new CreateTodo.Command
        {
            Title = "aa", // Invalid (must be >= 3)
            Description = "Valid Description",
            Priority = TodoPriority.Medium
        };

        var response = await client.PostAsJsonAsync(Routes.Todos.Create, command, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateTodo_WhenDescriptionTooShort_ReturnsValidationError()
    {
        var client = CreateAuthenticatedClient();

        var command = new CreateTodo.Command
        {
            Title = "Valid Title",
            Description = "aa", // Invalid (must be >= 3)
            Priority = TodoPriority.High
        };

        var response = await client.PostAsJsonAsync(Routes.Todos.Create, command, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateTodo_WhenDescriptionTooLong_ReturnsValidationError()
    {
        var client = CreateAuthenticatedClient();

        var longDescription = new string('a', Todo.MaxDescriptionLength + 1);

        var command = new CreateTodo.Command
        {
            Title = "Valid Title",
            Description = longDescription,
            Priority = TodoPriority.High
        };

        var response = await client.PostAsJsonAsync(Routes.Todos.Create, command, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateTodo_WhenUserIsNotAuthenticated_ReturnsUnauthorized()
    {
        var client = CreateUnauthenticatedClient();

        var command = new CreateTodo.Command
        {
            Title = "Valid Title",
            Description = "Valid Description",
            Priority = TodoPriority.Low
        };

        var response = await client.PostAsJsonAsync(Routes.Todos.Create, command, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}