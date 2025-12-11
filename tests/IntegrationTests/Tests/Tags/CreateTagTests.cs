using System.Net;
using Api.Features.Tags.Endpoints;
using Api.Features.Tags.Model;
using FluentAssertions;
using IntegrationTests.Extensions;

namespace IntegrationTests.Tests.Tags;

public sealed class CreateTagTests(TestingFixture fixture) : TestingBase(fixture)
{
    [Fact]
    public async Task CreateTag_Should_Return401_When_Unauthenticated()
    {
        // Arrange
        var unauthenticated = CreateUnauthenticatedClient();

        // Act
        var response = await unauthenticated.CreateTagAsync(
            new CreateTag.Command
            {
                TagName = "NewTag"
            },
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateTag_Should_Return201_When_TagIsCreated()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        var userId = CurrentUserId;

        // Act
        var response = await client.CreateTagAsync(
            new CreateTag.Command
            {
                TagName = "MyTag"
            },
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var uriLocation = response.Headers.Location;
        uriLocation.Should().NotBeNull();
        uriLocation.ToString().Should().StartWith("api/todos/");

        var tagId = TagId.From(Guid.Parse(uriLocation.ToString().Split('/').Last()));
        tagId.Should().NotBeNull();

        var createdTag = await GetDbContext().Tags
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == tagId, CurrentCancellationToken);

        createdTag.Should().NotBeNull();
        createdTag.Name.Should().Be("MyTag");
        createdTag.UserId.Should().Be(userId);
    }

    [Fact]
    public async Task CreateTag_Should_Return409_When_TagAlreadyExists()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        var userId = CurrentUserId;

        var existingTag = new Tag("ExistingName", userId);

        await using var dbContext = GetDbContext();
        dbContext.Tags.Add(existingTag);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        // Act
        var response = await client.CreateTagAsync(
            new CreateTag.Command
            {
                TagName = "ExistingName"
            },
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        await response.ContainsErrorCode("Tag.AlreadyExists", CurrentCancellationToken);
    }

    [Fact]
    public async Task CreateTag_Should_Return400_When_NameIsTooShort()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();

        // Act
        var response = await client.CreateTagAsync(
            new CreateTag.Command
            {
                TagName = "ab" // less than MinNameLength = 3
            },
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        await response.ContainsErrorCode("Tag.InvalidName", CurrentCancellationToken);
    }

    [Fact]
    public async Task CreateTag_Should_Return400_When_NameIsTooLong()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();

        var tooLongName = new string('x', Tag.MaxNameLength + 1);

        // Act
        var response = await client.CreateTagAsync(
            new CreateTag.Command
            {
                TagName = tooLongName
            },
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        await response.ContainsErrorCode("Tag.InvalidName", CurrentCancellationToken);
    }
}