using System.Net;
using Api.Features.Todos.Endpoints;
using Api.Features.Todos.Model;
using FluentAssertions;
using IntegrationTests.Extensions;

namespace IntegrationTests.Tests.Todos;

public sealed class CreateTodoTests(TestingFixture fixture) : TestingBase(fixture)
{
    private static CreateTodo.Command CreateValidCommand()
    {
        return new CreateTodo.Command
        {
            Title = "Valid Title",
            Description = "Valid Description",
            Priority = TodoPriority.Medium
        };
    }

    [Fact]
    public async Task CreateTodo_Should_Return401_When_Unauthenticated()
    {
        // Arrange
        var client = CreateUnauthenticatedClient();
        var command = CreateValidCommand();

        // Act
        var response = await client.CreateTodoAsync(
            command,
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateTodo_Should_Return400_When_TitleIsTooShort()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var command = new CreateTodo.Command
        {
            Title = "Hi",
            Description = "Valid description",
            Priority = TodoPriority.Low
        };

        // Act
        var response = await client.CreateTodoAsync(
            command,
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        await response.ContainsErrorCode("Todo.InvalidDescription", CurrentCancellationToken);
    }

    [Fact]
    public async Task CreateTodo_Should_Return400_When_TitleIsTooLong()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var longTitle = new string('a', Todo.MaxTitleLength + 1);

        var command = new CreateTodo.Command
        {
            Title = longTitle,
            Description = "Valid description",
            Priority = TodoPriority.Medium
        };

        // Act
        var response = await client.CreateTodoAsync(
            command,
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        await response.ContainsErrorCode("Todo.InvalidDescription", CurrentCancellationToken);
    }

    [Fact]
    public async Task CreateTodo_Should_Return400_When_DescriptionIsTooShort()
    {
        // Arrange
        var client = CreateAuthenticatedClient();

        var command = new CreateTodo.Command
        {
            Title = "Valid Title",
            Description = "Yo",
            Priority = TodoPriority.High
        };

        // Act
        var response = await client.CreateTodoAsync(
            command,
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        await response.ContainsErrorCode("Todo.InvalidDescription", CurrentCancellationToken);
    }

    [Fact]
    public async Task CreateTodo_Should_Return400_When_DescriptionIsTooLong()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var longDesc = new string('x', Todo.MaxDescriptionLength + 1);

        var command = new CreateTodo.Command
        {
            Title = "Valid Title",
            Description = longDesc,
            Priority = TodoPriority.Medium
        };

        // Act
        var response = await client.CreateTodoAsync(
            command,
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        await response.ContainsErrorCode("Todo.InvalidDescription", CurrentCancellationToken);
    }

    [Fact]
    public async Task CreateTodo_Should_Return201_And_CreateTodo()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var command = CreateValidCommand();

        // Act
        var response = await client.CreateTodoAsync(
            command,
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.MapTo<CreateTodo.Response>(CurrentCancellationToken);
        body.TodoId.Should().NotBe(Guid.Empty);

        var location = response.Headers.Location;
        location.Should().NotBeNull();

        location!
            .ToString()
            .Split('/')
            .Last()
            .Should()
            .Be(body.TodoId.ToString());

        var created = await DbContext.Todos
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == TodoId.From(body.TodoId), CurrentCancellationToken);

        created.Should().NotBeNull();
        created!.Title.Should().Be(command.Title);
        created.Description.Should().Be(command.Description);
        created.Priority.Should().Be(command.Priority);
    }
}