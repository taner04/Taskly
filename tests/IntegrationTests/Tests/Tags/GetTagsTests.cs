using System.Net;
using System.Net.Http.Json;
using Api.Features.Tags.Model;
using Api.Shared.Api;
using Api.Shared.Dtos.Tags;
using IntegrationTests.Extensions;

namespace IntegrationTests.Tests.Tags;

public class GetTagsTests(TestingFixture fixture) : TestingBase(fixture)
{
    private static string Url => Routes.Tags.GetTags;

    [Fact]
    public async Task GetTags_WhenUserHasTags_ReturnsOnlyUserTags()
    {
        var client = CreateAuthenticatedClient();
        var userId = client.GetUserId();

        var tag1 = new Tag("Work", userId);
        var tag2 = new Tag("Personal", userId);
        var otherTag = new Tag("Foreign", "auth0|other");

        DbContext.Tags.AddRange(tag1, tag2, otherTag);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var response = await client.GetAsync(Url, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var tags = await response.Content.ReadFromJsonAsync<List<TagDto>>(CurrentCancellationToken);

        Assert.NotNull(tags);
        Assert.Equal(2, tags!.Count);
        Assert.Contains(tags, t => t.Name == "Work");
        Assert.Contains(tags, t => t.Name == "Personal");
        Assert.DoesNotContain(tags, t => t.Name == "Foreign");
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
    public async Task GetTags_WhenUserNotAuthenticated_ReturnsUnauthorized()
    {
        var client = CreateUnauthenticatedClient();

        var response = await client.GetAsync(Url, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}