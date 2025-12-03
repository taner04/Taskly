using System.Net;
using System.Net.Http.Json;
using Api.Features.Tags;
using Api.Features.Tags.Model;
using Api.Shared.Features.Api;
using IntegrationTests.Extensions;

namespace IntegrationTests.Tests.Tags;

public sealed class CreateTagTests(TestingFixture fixture) : TestingBase(fixture)
{
    private const string Url = Routes.Tags.Create;

    [Fact]
    public async Task CreateTag_WhenValidRequest_ReturnsOkAndCreatesTag()
    {
        var client = CreateAuthenticatedClient();
        var userId = client.GetUserId();

        var requestBody = new CreateTag.Command
        {
            TagName = "MyTag"
        };

        var response = await client.PostAsJsonAsync(Url, requestBody, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var tag = await DbContext.Tags.AsNoTracking()
            .SingleAsync(t => t.Name == "MyTag" && t.UserId == userId, CurrentCancellationToken);

        Assert.Equal("MyTag", tag.Name);
        Assert.Equal(userId, tag.UserId);
    }

    [Fact]
    public async Task CreateTag_WhenTagAlreadyExists_ReturnsBadRequest()
    {
        var client = CreateAuthenticatedClient();
        var userId = client.GetUserId();

        var existing = Tag.TryCreate("ExistingTag", userId).Value;
        DbContext.Tags.Add(existing);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var requestBody = new CreateTag.Command
        {
            TagName = "ExistingTag"
        };

        var response = await client.PostAsJsonAsync(Url, requestBody, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateTag_WhenTagNameIsTooShort_ReturnsBadRequest()
    {
        var client = CreateAuthenticatedClient();

        var requestBody = new CreateTag.Command
        {
            TagName = "ab" // Invalid (must be >= 3)
        };

        var response = await client.PostAsJsonAsync(Url, requestBody, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateTag_WhenTagNameIsTooLong_ReturnsBadRequest()
    {
        var client = CreateAuthenticatedClient();

        var longTagName = new string('a', Tag.MaxNameLength + 1);
        var requestBody = new CreateTag.Command
        {
            TagName = longTagName
        };

        var response = await client.PostAsJsonAsync(Url, requestBody, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateTag_WhenTagNameIsEmpty_ReturnsBadRequest()
    {
        var client = CreateAuthenticatedClient();

        var requestBody = new CreateTag.Command
        {
            TagName = ""
        };

        var response = await client.PostAsJsonAsync(Url, requestBody, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateTag_WhenUserIsNotAuthenticated_ReturnsUnauthorized()
    {
        var client = CreateUnauthenticatedClient();

        var requestBody = new CreateTag.Command
        {
            TagName = "UnauthorizedTag"
        };

        var response = await client.PostAsJsonAsync(Url, requestBody, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}