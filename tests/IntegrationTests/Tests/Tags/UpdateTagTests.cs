using System.Net;
using System.Net.Http.Json;
using Api.Features.Shared.Api;
using Api.Features.Tags;
using Api.Features.Tags.Model;
using IntegrationTests.Extensions;

namespace IntegrationTests.Tests.Tags;

public sealed class UpdateTagTests(TestingFixture fixture) : TestingBase(fixture)
{
    [Fact]
    public async Task UpdateTag_WhenTagExistsAndBelongsToUser_UpdatesTag()
    {
        var client = CreateAuthenticatedClient();
        var userId = client.GetUserId();

        var tag = Tag.TryCreate("Original", userId).Value;
        DbContext.Tags.Add(tag);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var url = Routes.Tags.Update.ParseTagRoute(tag.Id.Value);

        var body = new UpdateTag.Command.CommandBody
        {
            NewName = "UpdatedName"
        };

        var response = await client.PutAsJsonAsync(url, body, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var updated = await DbContext.Tags.AsNoTracking()
            .SingleAsync(t => t.Id == tag.Id, CurrentCancellationToken);

        Assert.Equal("UpdatedName", updated.Name);
    }

    [Fact]
    public async Task UpdateTag_WhenTagDoesNotExist_ReturnsNotFound()
    {
        var client = CreateAuthenticatedClient();

        var url = Routes.Tags.Update.ParseTagRoute(Guid.NewGuid());

        var body = new UpdateTag.Command.CommandBody
        {
            NewName = "NewName"
        };

        var response = await client.PutAsJsonAsync(url, body, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateTag_WhenTagBelongsToAnotherUser_ReturnsNotFound()
    {
        var client = CreateAuthenticatedClient();

        var tag = Tag.TryCreate("OtherUsersTag", "auth0|someoneElse").Value;

        DbContext.Tags.Add(tag);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var url = Routes.Tags.Update.ParseTagRoute(tag.Id.Value);

        var body = new UpdateTag.Command.CommandBody
        {
            NewName = "ShouldFail"
        };

        var response = await client.PutAsJsonAsync(url, body, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateTag_WhenNewNameIsTooShort_ReturnsBadRequest()
    {
        var client = CreateAuthenticatedClient();
        var userId = client.GetUserId();

        var tag = Tag.TryCreate("Original", userId).Value;
        DbContext.Tags.Add(tag);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var url = Routes.Tags.Update.ParseTagRoute(tag.Id.Value);

        var body = new UpdateTag.Command.CommandBody
        {
            NewName = "ab" // Invalid (must be >= 3)
        };

        var response = await client.PutAsJsonAsync(url, body, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateTag_WhenNewNameIsTooLong_ReturnsBadRequest()
    {
        var client = CreateAuthenticatedClient();
        var userId = client.GetUserId();

        var tag = Tag.TryCreate("Original", userId).Value;
        DbContext.Tags.Add(tag);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var url = Routes.Tags.Update.ParseTagRoute(tag.Id.Value);

        var longName = new string('a', Tag.MaxNameLength + 1);
        var body = new UpdateTag.Command.CommandBody
        {
            NewName = longName
        };

        var response = await client.PutAsJsonAsync(url, body, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateTag_WhenUserIsNotAuthenticated_ReturnsUnauthorized()
    {
        var client = CreateUnauthenticatedClient();

        var url = Routes.Tags.Update.ParseTagRoute(Guid.NewGuid());

        var body = new UpdateTag.Command.CommandBody
        {
            NewName = "Name"
        };

        var response = await client.PutAsJsonAsync(url, body, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}