using System.Net;
using System.Net.Http.Json;
using Api.Features.Tags.Endpoints;
using Api.Features.Tags.Model;
using Api.Shared.Api;
using IntegrationTests.Extensions;

namespace IntegrationTests.Tests.Tags;

public class UpdateTagTests(TestingFixture fixture) : TestingBase(fixture)
{
    private static string GetRoute(
        Guid tagId)
    {
        return Routes.Tags.Update.Replace("{tagId:guid}", tagId.ToString());
    }

    [Fact]
    public async Task UpdateTag_WhenValid_UpdatesTag()
    {
        var client = CreateAuthenticatedClient();
        var userId = client.GetUserId();

        var tag = new Tag("Work", userId);
        DbContext.Tags.Add(tag);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var url = GetRoute(tag.Id.Value);

        var command = new UpdateTag.Command.CommandBody
        {
            NewName = "UpdatedName"
        };

        var response = await client.PutAsJsonAsync(url, command, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var updated = await DbContext.Tags
            .AsNoTracking()
            .SingleOrDefaultAsync(t => t.Id == tag.Id, CurrentCancellationToken);

        Assert.NotNull(updated);
        Assert.Equal("UpdatedName", updated!.Name);
    }

    [Fact]
    public async Task UpdateTag_WhenTagNotFound_ReturnsBadRequest()
    {
        var client = CreateAuthenticatedClient();

        var missingId = TagId.From(Guid.NewGuid());
        var url = GetRoute(missingId.Value);

        var command = new UpdateTag.Command.CommandBody
        {
            NewName = "Whatever"
        };

        var response = await client.PutAsJsonAsync(url, command, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problem = await response.ReadProblemAsync();

        Assert.Equal("Tag.NotFound", problem.ErrorCode);
        Assert.Equal("CouldNot find tag.", problem.Title);
        Assert.Contains(missingId.Value.ToString(), problem.Detail);
        Assert.Empty(problem.Errors);
    }

    [Fact]
    public async Task UpdateTag_WhenTagBelongsToAnotherUser_ReturnsBadRequest()
    {
        var client = CreateAuthenticatedClient();

        var foreignTag = new Tag("OtherTag", "auth0|someoneElse");
        DbContext.Tags.Add(foreignTag);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var url = GetRoute(foreignTag.Id.Value);

        var command = new UpdateTag.Command.CommandBody
        {
            NewName = "NewName"
        };

        var response = await client.PutAsJsonAsync(url, command, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problem = await response.ReadProblemAsync();

        Assert.Equal("Tag.NotFound", problem.ErrorCode);
        Assert.Equal("CouldNot find tag.", problem.Title);
        Assert.Empty(problem.Errors);
    }

    [Fact]
    public async Task UpdateTag_WhenNewNameInvalid_ReturnsInvalidNameError()
    {
        var client = CreateAuthenticatedClient();
        var userId = client.GetUserId();

        var tag = new Tag("Work", userId);
        DbContext.Tags.Add(tag);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var url = GetRoute(tag.Id.Value);

        var command = new UpdateTag.Command.CommandBody
        {
            NewName = "ab"
        };

        var response = await client.PutAsJsonAsync(url, command, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problem = await response.ReadProblemAsync();

        Assert.Equal("Tag.InvalidName", problem.ErrorCode);
        Assert.Equal("The tag name is invalid.", problem.Title);
        Assert.Empty(problem.Errors);
    }

    [Fact]
    public async Task UpdateTag_WhenUserUnauthenticated_ReturnsUnauthorized()
    {
        var client = CreateUnauthenticatedClient();

        var url = GetRoute(Guid.NewGuid());

        var command = new UpdateTag.Command.CommandBody
        {
            NewName = "Test"
        };

        var response = await client.PutAsJsonAsync(url, command, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}