using System.Net;
using System.Net.Http.Json;
using Api.Features.Shared.Dtos.Tags;
using Api.Features.Tags;
using Api.Features.Tags.Model;
using IntegrationTests.Extensions;

namespace IntegrationTests.Tests.Tags;

public sealed class GetTagsTests(TestingFixture fixture) : TestingBase(fixture)
{
    private const string Url = ApiRoutes.Tags.GetTags;

    [Fact]
    public async Task GetTags_WhenUserHasTags_ReturnsTagsOnlyForUser()
    {
        var client = CreateAuthenticatedClient();
        var userId = client.GetUserId();

        DbContext.Tags.Add(Tag.TryCreate("TagA", userId).Value);
        DbContext.Tags.Add(Tag.TryCreate("TagB", userId).Value);
        DbContext.Tags.Add(Tag.TryCreate("OtherUsersTag", "another-user").Value);

        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var response = await client.GetAsync(Url, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var tags = await response.Content.ReadFromJsonAsync<List<TagDto>>(CurrentCancellationToken);

        Assert.NotNull(tags);
        Assert.Equal(2, tags!.Count);
        Assert.All(tags, t => Assert.Equal(userId, t.UserId));
    }

    [Fact]
    public async Task GetTags_WhenUserHasNoTags_ReturnsEmptyList()
    {
        var client = CreateAuthenticatedClient();

        var response = await client.GetAsync(Url, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var tags = await response.Content.ReadFromJsonAsync<List<TagDto>>(CurrentCancellationToken);

        Assert.NotNull(tags);
        Assert.Empty(tags);
    }

    [Fact]
    public async Task GetTags_WhenTagsBelongToOtherUsers_ReturnsEmptyList()
    {
        var client = CreateAuthenticatedClient();

        DbContext.Tags.Add(Tag.TryCreate("OtherUser1Tag", "auth0|user1").Value);
        DbContext.Tags.Add(Tag.TryCreate("OtherUser2Tag", "auth0|user2").Value);

        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var response = await client.GetAsync(Url, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var tags = await response.Content.ReadFromJsonAsync<List<TagDto>>(CurrentCancellationToken);

        Assert.NotNull(tags);
        Assert.Empty(tags);
    }

    [Fact]
    public async Task GetTags_WhenUserIsNotAuthenticated_ReturnsUnauthorized()
    {
        var client = CreateUnauthenticatedClient();

        var response = await client.GetAsync(Url, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}