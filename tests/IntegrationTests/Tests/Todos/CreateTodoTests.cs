using System.Net;
using Api.Features.Todos.Endpoints;
using Api.Features.Todos.Model;

namespace IntegrationTests.Tests.Todos;

public sealed class CreateTodoTests(TestingFixture fixture) : TestingBase(fixture)
{
    [Fact]
    public async Task CreateTodo_UnauthenticatedClient_Returns_Unauthorized()
    {
        var client = CreateUnauthenticatedClient();

        var cmd = new CreateTodo.Command
        {
            Title = "Title",
            Description = "Description",
            Priority = TodoPriority.Medium
        };

        var apiResponse = await client.CreateTodoAsync(cmd, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, apiResponse.StatusCode);
    }
}