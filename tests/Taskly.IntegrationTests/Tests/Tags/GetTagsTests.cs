using Taskly.WebApi.Common.Shared.Dtos;
using Taskly.WebApi.Common.Shared.Pagination;
using Taskly.WebApi.Features.Tags.Endpoints;

namespace Taskly.IntegrationTests.Tests.Tags;

public sealed class GetTagsTests(TestingFixture fixture) : TestingBase(fixture)
{
    [Fact]
    public async Task GetTags_Should_Return401_When_Unauthenticated()
    {
        // Arrange
        var unauthenticatedClient = CreateUnauthenticatedClient();

        // Act
        var response = await unauthenticatedClient.GetTagsAsync(
            new GetTags.Query(0, 10),
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetTags_Should_ReturnEmptyList_When_UserHasNoTags()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();

        // Ensure DB is empty for this user
        var userId = CurrentUserId;
        var existing = await GetDbContext().Tags
            .Where(t => t.UserId == userId)
            .ToListAsync(CurrentCancellationToken);

        GetDbContext().Tags.RemoveRange(existing);
        await GetDbContext().SaveChangesAsync(CurrentCancellationToken);

        // Act
        var response = await client.GetTagsAsync(
            new GetTags.Query(0, 10),
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.MapTo<PaginationResult<TagDto>>(CurrentCancellationToken);

        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task GetTags_Should_ReturnTags_OnlyForAuthenticatedUser()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        var userId = CurrentUserId;

        // Tags belonging to the authenticated user
        var tag1 = TagFactory.Create("Work", userId);
        var tag2 = TagFactory.Create("Personal", userId);

        // Tag belonging to a different user — SHOULD NOT appear
        var tagOtherUser = TagFactory.Create("OtherUserTag", UserId.EmptyId);

        await using var dbContext = GetDbContext();
        dbContext.Tags.AddRange(tag1, tag2, tagOtherUser);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        // Act
        var response = await client.GetTagsAsync(
            new GetTags.Query(0, 10),
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.MapTo<PaginationResult<TagDto>>(CurrentCancellationToken);

        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);

        result.Items.Select(t => t.Name).Should().BeEquivalentTo("Work", "Personal");
        result.Items.Should().OnlyContain(t => t.Name != "OtherUserTag");
    }
}