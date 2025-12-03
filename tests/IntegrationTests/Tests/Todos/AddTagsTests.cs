using System.Net;
using System.Net.Http.Json;
using Api.Features.Tags.Model;
using Api.Features.Todos.Model;
using Api.Shared.Features.Api;
using IntegrationTests.Extensions;

namespace IntegrationTests.Tests.Todos;

public class AddTagsTests(TestingFixture fixture) : TestingBase(fixture)
{
    [Fact]
    public async Task AddTags_WhenTodoAndTagsExistAndBelongToUser_AddsTagsToTodo()
    {
        var client = CreateAuthenticatedClient();
        var userId = client.GetUserId();

        var todo = Todo.TryCreate("Todo1", "Desc", TodoPriority.Medium, userId).Value;

        var tagA = Tag.TryCreate("TagA", userId).Value;
        var tagB = Tag.TryCreate("TagB", userId).Value;

        DbContext.Todos.Add(todo);
        DbContext.Tags.AddRange(tagA, tagB);

        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var url = Routes.Todos.AddTags.ParseTodoRoute(todo.Id.Value);

        var body = new AddTags.Command.CommandBody
        {
            TagIds = [tagA.Id, tagB.Id]
        };

        var response = await client.PostAsJsonAsync(url, body, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var updated = await DbContext.Todos
            .Include(t => t.Tags)
            .AsNoTracking()
            .SingleAsync(t => t.Id == todo.Id, CurrentCancellationToken);

        Assert.Equal(2, updated.Tags.Count);
        Assert.Contains(updated.Tags, t => t.Id == tagA.Id);
        Assert.Contains(updated.Tags, t => t.Id == tagB.Id);
    }

    [Fact]
    public async Task AddTags_WhenTodoDoesNotExist_ReturnsNotFound()
    {
        var client = CreateAuthenticatedClient();

        var url = Routes.Todos.AddTags.ParseTodoRoute(Guid.NewGuid());

        var body = new AddTags.Command.CommandBody
        {
            TagIds = [TagId.From(Guid.NewGuid())]
        };

        var response = await client.PostAsJsonAsync(url, body, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AddTags_WhenNoTagsMatchProvidedIds_ReturnsNotFound()
    {
        var client = CreateAuthenticatedClient();
        var userId = client.GetUserId();

        // Only todo exists
        var todo = Todo.TryCreate("TodoX", "Desc", TodoPriority.Medium, userId).Value;
        DbContext.Todos.Add(todo);

        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var url = Routes.Todos.AddTags.ParseTodoRoute(todo.Id.Value);

        var body = new AddTags.Command.CommandBody
        {
            TagIds = [TagId.From(Guid.NewGuid())]
        };

        var response = await client.PostAsJsonAsync(url, body, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AddTags_WhenTodoBelongsToAnotherUser_ReturnsNotFound()
    {
        var client = CreateAuthenticatedClient();

        var todo = Todo.TryCreate("Todo", "Desc", TodoPriority.Medium, "auth0|otherUser").Value;
        var tag = Tag.TryCreate("Tag", client.GetUserId()).Value;

        DbContext.Todos.Add(todo);
        DbContext.Tags.Add(tag);

        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var url = Routes.Todos.AddTags.ParseTodoRoute(todo.Id.Value);

        var body = new AddTags.Command.CommandBody
        {
            TagIds = [tag.Id]
        };

        var response = await client.PostAsJsonAsync(url, body, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AddTags_WhenTagsBelongToAnotherUser_ReturnsNotFound()
    {
        var client = CreateAuthenticatedClient();
        var userId = client.GetUserId();

        var todo = Todo.TryCreate("Todo", "Desc", TodoPriority.Medium, userId).Value;

        var externalTag = Tag.TryCreate("ForeignTag", "auth0|otherUser").Value;

        DbContext.Todos.Add(todo);
        DbContext.Tags.Add(externalTag);

        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var url = Routes.Todos.AddTags.ParseTodoRoute(todo.Id.Value);

        var body = new AddTags.Command.CommandBody
        {
            TagIds = [externalTag.Id]
        };

        var response = await client.PostAsJsonAsync(url, body, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AddTags_WhenUserNotAuthenticated_ReturnsUnauthorized()
    {
        var client = CreateUnauthenticatedClient();

        var url = Routes.Todos.AddTags.ParseTodoRoute(Guid.NewGuid());

        var body = new AddTags.Command.CommandBody
        {
            TagIds = [TagId.From(Guid.NewGuid())]
        };

        var response = await client.PostAsJsonAsync(url, body, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}