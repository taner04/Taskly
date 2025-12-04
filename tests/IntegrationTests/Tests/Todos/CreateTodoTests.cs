using System.Net;
using System.Net.Http.Json;
using Api.Features.Todos.Endpoints;
using Api.Features.Todos.Model;
using Api.Shared.Api;
using IntegrationTests.Extensions;

namespace IntegrationTests.Tests.Todos;

public class CreateTodoTests(TestingFixture fixture) : TestingBase(fixture)
{
    private const string Url = Routes.Todos.Create;

    [Fact]
    public async Task CreateTodo_WhenValid_ReturnsCreatedAndPersists()
    {
        var client = CreateAuthenticatedClient();

        var body = new CreateTodo.Command
        {
            Title = "My Task",
            Description = "Some description",
            Priority = TodoPriority.Medium
        };

        var response = await client.PostAsJsonAsync(Url, body, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var dto = await response.Content.ReadFromJsonAsync<CreateTodo.Dto>(CurrentCancellationToken);
        Assert.NotNull(dto);

        var exists = await DbContext.Todos
            .AsNoTracking()
            .AnyAsync(t => t.Id == TodoId.From(dto.TodoId), CurrentCancellationToken);

        Assert.True(exists);
    }

    [Fact]
    public async Task CreateTodo_WhenTitleTooShort_ReturnsValidationError()
    {
        var client = CreateAuthenticatedClient();

        var body = new CreateTodo.Command
        {
            Title = "Hi", // invalid, < 3
            Description = "Desc",
            Priority = TodoPriority.High
        };

        var response = await client.PostAsJsonAsync(Url, body, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problem = await response.ReadProblemAsync();
        Assert.Equal("Todo.InvalidDescription", problem.ErrorCode);
    }

    [Fact]
    public async Task CreateTodo_WhenTitleTooLong_ReturnsValidationError()
    {
        var client = CreateAuthenticatedClient();

        var body = new CreateTodo.Command
        {
            Title = new string('x', 101), // invalid > 100
            Description = "Valid description",
            Priority = TodoPriority.Low
        };

        var response = await client.PostAsJsonAsync(Url, body, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problem = await response.ReadProblemAsync();
        Assert.Equal("Todo.InvalidDescription", problem.ErrorCode);
    }

    [Fact]
    public async Task CreateTodo_WhenUserNotAuthenticated_ReturnsUnauthorized()
    {
        var client = CreateUnauthenticatedClient();

        var body = new CreateTodo.Command
        {
            Title = "Task1",
            Description = "Desc",
            Priority = TodoPriority.Low
        };

        var response = await client.PostAsJsonAsync(Url, body, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}