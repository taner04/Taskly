using System.Net;
using Api.Features.Tags.Endpoints;
using Api.Features.Tags.Model;
using FluentAssertions;
using IntegrationTests.Extensions;

namespace IntegrationTests.Tests.Tags;

public sealed class UpdateTagTests(TestingFixture fixture) : TestingBase(fixture)
{
    private static Tag CreateTag(string name, string userId)
    {
        return new Tag(name, userId);
    }

    [Fact]
    public async Task UpdateTag_Should_Return401_When_Unauthenticated()
    {
        // Arrange
        var unauthenticated = CreateUnauthenticatedClient();
        var randomId = TagId.From(Guid.NewGuid());

        // Act
        var response = await unauthenticated.UpdateTagAsync(
            randomId,
            new UpdateTag.Command.CommandBody
            {
                NewName = "UpdatedName"
            },
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateTag_Should_Return404_When_TagDoesNotExist()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var randomId = TagId.From(Guid.NewGuid());

        // Act
        var response = await client.UpdateTagAsync(
            randomId,
            new UpdateTag.Command.CommandBody
            {
                NewName = "UpdatedName"
            },
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        await response.ContainsErrorCode("Tag.NotFound", CurrentCancellationToken);
    }

    [Fact]
    public async Task UpdateTag_Should_Return400_When_NewNameIsInvalid()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var userId = GetCurrentUserId();

        var tag = CreateTag("OriginalName", userId);

        DbContext.Tags.Add(tag);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        // Act
        var response = await client.UpdateTagAsync(
            tag.Id,
            new UpdateTag.Command.CommandBody
            {
                NewName = "ab" // Invalid: < min length of 3
            },
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        await response.ContainsErrorCode("Tag.InvalidName", CurrentCancellationToken);
    }

    [Fact]
    public async Task UpdateTag_Should_Return200_And_UpdateTheTag()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var userId = GetCurrentUserId();

        var tag = CreateTag("BeforeRename", userId);

        DbContext.Tags.Add(tag);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var newName = "AfterRename";

        // Act
        var response = await client.UpdateTagAsync(
            tag.Id,
            new UpdateTag.Command.CommandBody
            {
                NewName = newName
            },
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updated = await DbContext.Tags
            .AsNoTracking()
            .FirstAsync(t => t.Id == tag.Id, CurrentCancellationToken);

        updated.Name.Should().Be(newName);
    }
}