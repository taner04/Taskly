using System.Net;
using System.Net.Http.Json;
using Api.Features.Shared.Dtos.Tags;
using Api.Features.Tags.Endpoints;
using Api.Features.Tags.Model;
using FluentAssertions;

namespace IntegrationTests.Tests.Tags;

public sealed class GetTagsTests(TestingFixture fixture) : TestingBase(fixture)
{
    private static Tag CreateTag(
        string name,
        UserId userId)
    {
        return new Tag(name, userId);
    }

    [Fact]
    public async Task GetTags_Should_Return401_When_Unauthenticated()
    {
        // Arrange
        var unauthenticatedClient = CreateUnauthenticatedClient();

        // Act
        var response = await unauthenticatedClient.GetTagsAsync(
            new GetTags.Query(),
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
            new GetTags.Query(),
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<List<TagDto>>(
            CurrentCancellationToken);

        result.Should().NotBeNull();
        result!.Should().BeEmpty();
    }

    [Fact]
    public async Task GetTags_Should_ReturnTags_OnlyForAuthenticatedUser()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        var userId = CurrentUserId;

        // Tags belonging to the authenticated user
        var tag1 = CreateTag("Work", userId);
        var tag2 = CreateTag("Personal", userId);

        // Tag belonging to a different user — SHOULD NOT appear
        var tagOtherUser = CreateTag("OtherUserTag", UserId.EmptyId);

        await using var dbContext = GetDbContext();
        dbContext.Tags.AddRange(tag1, tag2, tagOtherUser);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        // Act
        var response = await client.GetTagsAsync(
            new GetTags.Query(),
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.MapTo<List<TagDto>>(CurrentCancellationToken);

        result.Should().NotBeNull();
        result.Should().HaveCount(2);

        result.Select(t => t.Name).Should().BeEquivalentTo("Work", "Personal");
        result.Should().OnlyContain(t => t.Name != "OtherUserTag");
    }
}