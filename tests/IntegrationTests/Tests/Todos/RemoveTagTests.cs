using System.Net;
using Api.Features.Tags.Model;
using Api.Features.Todos.Model;
using Api.Shared.Features.Api;
using IntegrationTests.Extensions;

namespace IntegrationTests.Tests.Todos;

public class RemoveTagTests(TestingFixture fixture) : TestingBase(fixture)
{
    private static string GetRoute(
        Guid todoId,
        Guid tagId)
    {
        return Routes.Todos.RemoveTag
            .Replace("{todoId:guid}", todoId.ToString())
            .Replace("{tagId:guid}", tagId.ToString());
    }

    [Fact]
    public async Task RemoveTag_WhenTagExistsOnTodo_RemovesTag()
    {
        var client = CreateAuthenticatedClient();
        var userId = client.GetUserId();

        var todo = Todo.TryCreate("Task1", "Desc", TodoPriority.Medium, userId).Value;
        var tag = Tag.TryCreate("TagX", userId).Value;

        todo.Tags.Add(tag);

        DbContext.Todos.Add(todo);
        DbContext.Tags.Add(tag);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var url = GetRoute(todo.Id.Value, tag.Id.Value);

        var response = await client.DeleteAsync(url, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

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

        var todo = Todo.TryCreate("TodoX", "Desc", TodoPriority.Medium, userId).Value;
        DbContext.Todos.Add(todo);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var url = GetRoute(todo.Id.Value, Guid.NewGuid()); // tag does not exist on todo

        var response = await client.DeleteAsync(url, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task RemoveTag_WhenTodoBelongsToAnotherUser_ReturnsNotFound()
    {
        var client = CreateAuthenticatedClient();

        var todo = Todo.TryCreate("WrongTodo", "Desc", TodoPriority.Medium, "auth0|otherUser").Value;
        var tag = Tag.TryCreate("Tag1", client.GetUserId()).Value;

        todo.Tags.Add(tag);

        DbContext.Todos.Add(todo);
        DbContext.Tags.Add(tag);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var url = GetRoute(todo.Id.Value, tag.Id.Value);

        var response = await client.DeleteAsync(url, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task RemoveTag_WhenTagBelongsToAnotherUser_ReturnsNotFound()
    {
        var client = CreateAuthenticatedClient();
        var userId = client.GetUserId();

        var todo = Todo.TryCreate("TodoY", "Desc", TodoPriority.Medium, userId).Value;
        var foreignTag = Tag.TryCreate("Foreign", "auth0|otherUser").Value;

        DbContext.Todos.Add(todo);
        DbContext.Tags.Add(foreignTag);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var url = GetRoute(todo.Id.Value, foreignTag.Id.Value);

        var response = await client.DeleteAsync(url, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task RemoveTag_WhenUserIsNotAuthenticated_ReturnsUnauthorized()
    {
        var client = CreateUnauthenticatedClient();

        var url = GetRoute(Guid.NewGuid(), Guid.NewGuid());

        var response = await client.DeleteAsync(url, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}