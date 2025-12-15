using System.Net;
using FluentAssertions;
using IntegrationTests.Factories;

namespace IntegrationTests.Tests.Users;

public sealed class GetUsersTests(TestingFixture fixture) : TestingBase(fixture)
{
    private static User CreateUser(
        string email)
    {
        return User.Create(
            email,
            "auth|123");
    }

    [Fact]
    public async Task GetUsers_Should_Return401_When_Unauthenticated()
    {
        // Arrange
        var client = CreateUnauthenticatedClient();

        // Act
        var response = await client.GetUsersAsync(CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetUsers_Should_Return403_When_UserIsNotAdmin()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();

        // Act
        var response = await client.GetUsersAsync(CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetUsers_Should_ReturnUsers_When_CalledByAdmin()
    {
        // Arrange
        var admin = CreateAuthenticatedAdminClient();

        var u1 = CreateUser("a@test.com");
        var u2 = CreateUser("b@test.com");

        await using var db = GetDbContext();
        db.Users.Add(u1);
        db.Users.Add(u2);
        await db.SaveChangesAsync(CurrentCancellationToken);

        // Act
        var response = await admin.GetUsersAsync(CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var list = await response.MapTo<List<User>>(CurrentCancellationToken);

        list.Should().HaveCount(3); // Including +1 for the seeded user from TestingBase
        list.Select(u => u.Email).Should()
            .BeEquivalentTo("a@test.com", "b@test.com",
                UserFactory.Email); // UserFactory.Email is the seeded user email
    }
}