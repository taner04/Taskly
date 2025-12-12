using System.Net;
using FluentAssertions;
using IntegrationTests.Extensions;

namespace IntegrationTests.Tests.Users;

public sealed class RemoveUserTests(TestingFixture fixture) : TestingBase(fixture)
{
    private static User CreateUser(
        string email)
    {
        return User.Create(email, "auth|123");
    }

    [Fact]
    public async Task RemoveUser_Should_Return401_When_Unauthenticated()
    {
        // Arrange
        var client = CreateUnauthenticatedClient();
        var userId = UserId.From(Guid.NewGuid());

        // Act
        var response = await client.RemoveUserAsync(
            userId,
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RemoveUser_Should_Return403_When_CalledByNonAdmin()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        var userId = UserId.From(Guid.NewGuid());

        // Act
        var response = await client.RemoveUserAsync(
            userId,
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task RemoveUser_Should_Return404_When_UserDoesNotExist()
    {
        // Arrange
        var admin = CreateAuthenticatedAdminClient();
        var randomId = UserId.From(Guid.NewGuid());

        // Act
        var response = await admin.RemoveUserAsync(
            randomId,
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        await response.ContainsErrorCode("User.NotFound", CurrentCancellationToken);
    }

    [Fact]
    public async Task RemoveUser_Should_DeleteUser_When_CalledByAdmin()
    {
        // Arrange
        var admin = CreateAuthenticatedAdminClient();

        var user = CreateUser("delete@test.com");

        await using var dbContext = GetDbContext();
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        // Act
        var response = await admin.RemoveUserAsync(
            user.Id,
            CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var exists = await dbContext.Users
            .AnyAsync(u => u.Id == user.Id, CurrentCancellationToken);

        exists.Should().BeFalse();
    }
}