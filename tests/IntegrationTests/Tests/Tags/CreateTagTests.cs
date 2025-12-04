using System.Net;
using System.Net.Http.Json;
using Api.Features.Tags.Endpoints;
using Api.Features.Tags.Model;
using Api.Shared.Api;
using IntegrationTests.Extensions;

namespace IntegrationTests.Tests.Tags;

public class CreateTagTests(TestingFixture fixture) : TestingBase(fixture)
{
    private static string Url => Routes.Tags.Create;

    [Fact]
    public async Task CreateTag_WhenValid_ReturnsCreatedAndPersistsTag()
    {
        var client = CreateAuthenticatedClient();
        var userId = client.GetUserId();

        var command = new CreateTag.Command
        {
            TagName = "Work"
        };

        var response = await client.PostAsJsonAsync(Url, command, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var dto = await response.Content.ReadFromJsonAsync<CreateTag.Dto>(CurrentCancellationToken);
        Assert.NotNull(dto);

        var tag = await DbContext.Tags.AsNoTracking()
            .SingleOrDefaultAsync(t => t.Id == TagId.From(dto.TagId), CurrentCancellationToken);

        Assert.NotNull(tag);
        Assert.Equal("Work", tag!.Name);
        Assert.Equal(userId, tag.UserId);
    }

    [Fact]
    public async Task CreateTag_WhenTagAlreadyExists_ReturnsBadRequest()
    {
        var client = CreateAuthenticatedClient();
        var userId = client.GetUserId();

        // Arrange: pre-insert tag
        DbContext.Tags.Add(new Tag("Work", userId));
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var command = new CreateTag.Command
        {
            TagName = "Work"
        };

        var response = await client.PostAsJsonAsync(Url, command, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problem = await response.ReadProblemAsync();

        Assert.Equal("Tag.AlreadyExists", problem.ErrorCode);
        Assert.Equal("Tag already exists.", problem.Title);
        Assert.Equal("A tag with the name 'Work' already exists.", problem.Detail);

        Assert.Empty(problem.Errors);
    }

    [Fact]
    public async Task CreateTag_WhenNameTooShort_ReturnsInvalidNameException()
    {
        var client = CreateAuthenticatedClient();

        var command = new CreateTag.Command
        {
            TagName = "ab"
        };

        var response = await client.PostAsJsonAsync(Url, command, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problem = await response.ReadProblemAsync();

        Assert.Equal("Tag.InvalidName", problem.ErrorCode);
        Assert.Equal("The tag name is invalid.", problem.Title);

        Assert.Contains("2", problem.Detail);

        Assert.Empty(problem.Errors);
    }

    [Fact]
    public async Task CreateTag_WhenUserNotAuthenticated_ReturnsUnauthorized()
    {
        var client = CreateUnauthenticatedClient();

        var command = new CreateTag.Command
        {
            TagName = "Work"
        };

        var response = await client.PostAsJsonAsync(Url, command, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}