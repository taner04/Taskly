using System.Net;
using System.Net.Http.Json;
using Api.Features.Tags.Model;
using Api.Features.Todos.Endpoints;
using Api.Features.Todos.Model;
using Api.Shared.Api;
using IntegrationTests.Extensions;

namespace IntegrationTests.Tests.Todos;

public class AddTagsTests(TestingFixture fixture) : TestingBase(fixture)
{
    private static string GetRoute(
        Guid todoId)
    {
        return Routes.Todos.AddTags.Replace("{todoId:guid}", todoId.ToString());
    }

    [Fact]
    public async Task AddTags_WhenTodoExistsAndTagsExist_AddsTags()
    {
        var client = CreateAuthenticatedClient();
        var userId = client.GetUserId();

        var todo = new Todo("Task", "Desc", TodoPriority.Medium, userId);
        var tag1 = new Tag("Work", userId);
        var tag2 = new Tag("Urgent", userId);

        DbContext.Todos.Add(todo);
        DbContext.Tags.AddRange(tag1, tag2);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var body = new AddTags.Command.CommandBody
        {
            TagIds = [tag1.Id, tag2.Id]
        };

        var response = await client.PostAsJsonAsync(GetRoute(todo.Id.Value), body, CurrentCancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var updated = await DbContext.Todos
            .Include(t => t.Tags)
            .SingleAsync(t => t.Id == todo.Id, CurrentCancellationToken);

        Assert.Equal(2, updated.Tags.Count);
    }

    [Fact]
    public async Task AddTags_WhenTodoNotFound_ReturnsNotFound()
    {
        var client = CreateAuthenticatedClient();

        var body = new AddTags.Command.CommandBody
        {
            TagIds = [TagId.From(Guid.NewGuid())]
        };

        var response = await client.PostAsJsonAsync(GetRoute(Guid.NewGuid()), body, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problem = await response.ReadProblemAsync();
        Assert.Equal("Todo.NotFound", problem.ErrorCode);
    }

    [Fact]
    public async Task AddTags_WhenNoTagsFound_ReturnsNotFound()
    {
        var client = CreateAuthenticatedClient();
        var userId = client.GetUserId();

        var todo = new Todo("Task", "Desc", TodoPriority.Low, userId);

        DbContext.Todos.Add(todo);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var body = new AddTags.Command.CommandBody
        {
            TagIds = [TagId.From(Guid.NewGuid())]
        };

        var response = await client.PostAsJsonAsync(GetRoute(todo.Id.Value), body, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problem = await response.ReadProblemAsync();
        Assert.Equal("Tag.NotFound", problem.ErrorCode);
    }

    [Fact]
    public async Task AddTags_WhenTagsBelongToAnotherUser_ReturnsNotFound()
    {
        var client = CreateAuthenticatedClient();
        var userId = client.GetUserId();

        var todo = new Todo("XXXX", "YYY", TodoPriority.High, userId);
        var foreignTag = new Tag("Secret", "auth0|other");

        DbContext.Todos.Add(todo);
        DbContext.Tags.Add(foreignTag);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var body = new AddTags.Command.CommandBody
        {
            TagIds = [foreignTag.Id]
        };

        var response = await client.PostAsJsonAsync(GetRoute(todo.Id.Value), body, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var problem = await response.ReadProblemAsync();
        Assert.Equal("Tag.NotFound", problem.ErrorCode);
    }

    [Fact]
    public async Task AddTags_WhenTagsAlreadyExistOnTodo_DoesNotDuplicate()
    {
        var client = CreateAuthenticatedClient();
        var userId = client.GetUserId();

        var tag = new Tag("Home", userId);
        var todo = new Todo("Task", "Desc", TodoPriority.Medium, userId);
        todo.Tags.Add(tag);

        DbContext.Todos.Add(todo);
        DbContext.Tags.Add(tag);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var body = new AddTags.Command.CommandBody
        {
            TagIds = [tag.Id]
        };

        var response = await client.PostAsJsonAsync(GetRoute(todo.Id.Value), body, CurrentCancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var updated = await DbContext.Todos
            .Include(t => t.Tags)
            .SingleAsync(t => t.Id == todo.Id, CurrentCancellationToken);

        Assert.Single(updated.Tags);
    }

    [Fact]
    public async Task AddTags_WhenUserIsNotAuthenticated_ReturnsUnauthorized()
    {
        var client = CreateUnauthenticatedClient();

        var body = new AddTags.Command.CommandBody
        {
            TagIds = [TagId.From(Guid.NewGuid())]
        };

        var response = await client.PostAsJsonAsync(GetRoute(Guid.NewGuid()), body, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}