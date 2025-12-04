using System.Net;
using Api.Features.Tags.Model;
using Api.Features.Todos.Model;
using Api.Shared.Api;
using IntegrationTests.Extensions;

namespace IntegrationTests.Tests.Tags;

public class DeleteTagTests(TestingFixture fixture) : TestingBase(fixture)
{
    private static string GetRoute(
        Guid tagId)
    {
        return Routes.Tags.Remove.Replace("{tagId:guid}", tagId.ToString());
    }

    [Fact]
    public async Task DeleteTag_WhenTagExistsAndBelongsToUser_RemovesTagAndDetachesFromTodos()
    {
        var client = CreateAuthenticatedClient();
        var userId = client.GetUserId();

        var createdTag = new Tag("Work", userId);

        var todo = new Todo("Todo1", "Desc", TodoPriority.Medium, userId);
        todo.Tags.Add(createdTag);

        DbContext.Tags.Add(createdTag);
        DbContext.Todos.Add(todo);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var url = GetRoute(createdTag.Id.Value);

        var response = await client.DeleteAsync(url, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Assert.False(await DbContext.Tags.AsNoTracking()
            .AnyAsync(t => t.Id == createdTag.Id, CurrentCancellationToken));

        var updatedTodos = await DbContext.Todos
            .AsNoTracking()
            .Include(t => t.Tags)
            .Where(t => t.UserId == userId)
            .ToListAsync(CurrentCancellationToken);

        Assert.Contains(updatedTodos, t => t.Id == todo.Id && t.Tags.Count == 0);
    }

    [Fact]
    public async Task DeleteTag_WhenTagDoesNotExist_ReturnsBadRequestWithCorrectProblemDetails()
    {
        var client = CreateAuthenticatedClient();
        var missingTagId = TagId.From(Guid.NewGuid());

        var url = GetRoute(missingTagId.Value);

        var response = await client.DeleteAsync(url, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problem = await response.ReadProblemAsync();

        Assert.Equal("Tag.NotFound", problem.ErrorCode);
        Assert.Equal("CouldNot find tag.", problem.Title);
        Assert.Contains(missingTagId.Value.ToString(), problem.Detail);

        Assert.Empty(problem.Errors);
    }

    [Fact]
    public async Task DeleteTag_WhenTagBelongsToAnotherUser_ReturnsBadRequest()
    {
        var client = CreateAuthenticatedClient();

        var tag = new Tag("OtherTag", "auth0|other-user");

        DbContext.Tags.Add(tag);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var url = GetRoute(tag.Id.Value);

        var response = await client.DeleteAsync(url, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problem = await response.ReadProblemAsync();

        Assert.Equal("Tag.NotFound", problem.ErrorCode);
        Assert.Equal("CouldNot find tag.", problem.Title);
        Assert.Empty(problem.Errors);
    }

    [Fact]
    public async Task DeleteTag_WhenUserIsNotAuthenticated_ReturnsUnauthorized()
    {
        var client = CreateUnauthenticatedClient();
        var url = GetRoute(Guid.NewGuid());

        var response = await client.DeleteAsync(url, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}