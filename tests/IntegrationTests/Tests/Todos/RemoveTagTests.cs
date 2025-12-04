using System.Net;
using Api.Features.Tags.Model;
using Api.Features.Todos.Model;
using Api.Shared.Api;
using IntegrationTests.Extensions;

namespace IntegrationTests.Tests.Todos;

public class RemoveTagTests(TestingFixture fixture) : TestingBase(fixture)
{
    private static string GetRoute(
        Guid todoId,
        Guid tagId)
    {
        return Routes.Todos.RemoveTag
            .Replace("{todoId}", todoId.ToString())
            .Replace("{tagId}", tagId.ToString());
    }

    [Fact]
    public async Task RemoveTag_WhenTagExistsOnTodo_RemovesTag()
    {
        var client = CreateAuthenticatedClient();
        var userId = client.GetUserId();

        var todo = new Todo("Task1", "Desc", TodoPriority.Medium, userId);
        var tag = new Tag("TagX", userId);

        todo.Tags.Add(tag);

        DbContext.Todos.Add(todo);
        DbContext.Tags.Add(tag);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var url = GetRoute(todo.Id.Value, tag.Id.Value);

        var response = await client.DeleteAsync(url, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var updated = await DbContext.Todos
            .Include(t => t.Tags)
            .AsNoTracking()
            .SingleAsync(t => t.Id == todo.Id, CurrentCancellationToken);

        Assert.Empty(updated.Tags);
    }

    [Fact]
    public async Task RemoveTag_WhenTodoNotFound_ReturnsNotFound()
    {
        var client = CreateAuthenticatedClient();

        var url = GetRoute(Guid.NewGuid(), Guid.NewGuid());

        var response = await client.DeleteAsync(url, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task RemoveTag_WhenTagNotOnTodo_ReturnsNotFound()
    {
        var client = CreateAuthenticatedClient();
        var userId = client.GetUserId();

        var todo = new Todo("TodoX", "Desc", TodoPriority.Medium, userId);

        DbContext.Todos.Add(todo);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var url = GetRoute(todo.Id.Value, Guid.NewGuid());

        var response = await client.DeleteAsync(url, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task RemoveTag_WhenTagBelongsToOtherUser_ReturnsNotFound()
    {
        var client = CreateAuthenticatedClient();
        var userId = client.GetUserId();

        var todo = new Todo("TodoY", "Desc", TodoPriority.Medium, userId);
        var foreignTag = new Tag("OtherTag", "auth0|other");

        DbContext.Todos.Add(todo);
        DbContext.Tags.Add(foreignTag);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var url = GetRoute(todo.Id.Value, foreignTag.Id.Value);

        var response = await client.DeleteAsync(url, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task RemoveTag_WhenTodoBelongsToOtherUser_ReturnsNotFound()
    {
        var client = CreateAuthenticatedClient();
        var foreignUserId = "auth0|other";

        var todo = new Todo("OtherTodo", "Desc", TodoPriority.Medium, foreignUserId);
        var tag = new Tag("TagX", client.GetUserId());

        todo.Tags.Add(tag);

        DbContext.Todos.Add(todo);
        DbContext.Tags.Add(tag);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var url = GetRoute(todo.Id.Value, tag.Id.Value);

        var response = await client.DeleteAsync(url, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task RemoveTag_WhenUserNotAuthenticated_ReturnsUnauthorized()
    {
        var client = CreateUnauthenticatedClient();

        var url = GetRoute(Guid.NewGuid(), Guid.NewGuid());

        var response = await client.DeleteAsync(url, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}