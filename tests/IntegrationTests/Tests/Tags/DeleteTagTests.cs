using System.Net;
using System.Net.Http.Json;
using Api.Features.Tags.Domain;
using IntegrationTests.Extensions;

namespace IntegrationTests.Tests.Tags;

public class DeleteTagTests(TestingFixture fixture) : TestingBase(fixture)
{
    [Fact]
    public async Task DeleteTag_WhenTagExistsAndBelongsToUser_DeletesTag()
    {
        var client = CreateAuthenticatedClient();
        var userId = client.GetUserId();

        var tag = Tag.TryCreate("DeleteMe", userId).Value;
        DbContext.Tags.Add(tag);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var url = ApiRoutes.Tags.Delete.ParseTagRoute(tag.Id.Value);

        var response = await client.DeleteAsync(url, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var exists = await DbContext.Tags.AnyAsync(t => t.Id == tag.Id, CurrentCancellationToken);
        Assert.False(exists);
    }

    [Fact]
    public async Task DeleteTag_WhenTagDoesNotExist_ReturnsNotFound()
    {
        var client = CreateAuthenticatedClient();
        var url = ApiRoutes.Tags.Delete.ParseTagRoute(Guid.NewGuid());

        var response = await client.DeleteAsync(url, CurrentCancellationToken);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteTag_WhenTagBelongsToAnotherUser_ReturnsNotFound()
    {
        var client = CreateAuthenticatedClient();

        var tag = Tag.TryCreate("OtherUsersTag", "auth0|someoneElse").Value;
        DbContext.Tags.Add(tag);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var url = ApiRoutes.Tags.Delete.ParseTagRoute(tag.Id.Value);

        var response = await client.DeleteAsync(url, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteTag_WhenTagIsAssociatedWithTodos_RemovesTagFromAllTodos()
    {
        var client = CreateAuthenticatedClient();
        var userId = client.GetUserId();

        var tag = Tag.TryCreate("TagToDelete", userId).Value;
        DbContext.Tags.Add(tag);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        // Create todos and associate them with the tag
        var todo1 = Todo.TryCreate("Todo 1", "Description", TodoPriority.High, userId).Value;
        var todo2 = Todo.TryCreate("Todo 2", "Description", TodoPriority.Low, userId).Value;

        todo1.Tags.Add(tag);
        todo2.Tags.Add(tag);

        DbContext.Todos.AddRange(todo1, todo2);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var url = ApiRoutes.Tags.Delete.ParseTagRoute(tag.Id.Value);

        var response = await client.DeleteAsync(url, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Verify tag was deleted
        var tagExists = await DbContext.Tags.AnyAsync(t => t.Id == tag.Id, CurrentCancellationToken);
        Assert.False(tagExists);

        // Verify todos still exist but no longer have the tag
        var updatedTodo1 = await DbContext.Todos.AsNoTracking()
            .Include(t => t.Tags)
            .SingleAsync(t => t.Id == todo1.Id, CurrentCancellationToken);
        var updatedTodo2 = await DbContext.Todos.AsNoTracking()
            .Include(t => t.Tags)
            .SingleAsync(t => t.Id == todo2.Id, CurrentCancellationToken);

        Assert.Empty(updatedTodo1.Tags);
        Assert.Empty(updatedTodo2.Tags);
    }

    [Fact]
    public async Task DeleteTag_WhenUserIsNotAuthenticated_ReturnsUnauthorized()
    {
        var client = CreateUnauthenticatedClient();

        var url = ApiRoutes.Tags.Delete.ParseTagRoute(Guid.NewGuid());

        var response = await client.DeleteAsync(url, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
