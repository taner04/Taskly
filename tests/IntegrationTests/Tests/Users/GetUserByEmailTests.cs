using System.Net;
using FluentAssertions;
using IntegrationTests.Extensions;

namespace IntegrationTests.Tests.Users;

public sealed class GetUserByEmailTests(TestingFixture fixture) : TestingBase(fixture)
{
    private static User CreateUser(
        string email)
    {
        return User.Create(email, "auth|123");
    }

    [Fact]
    public async Task GetUserByEmail_Should_Return401_When_Unauthenticated()
    {
        // Arrange
        var client = CreateUnauthenticatedClient();
        const string email = "test@test.com";

        // Act
        var response = await client.GetUserByEmailAsync(email, CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetUserByEmail_Should_Return403_When_CalledByNonAdmin()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        const string email = "test@test.com";

        // Act
        var response = await client.GetUserByEmailAsync(email, CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetUserByEmail_Should_Return404_When_UserDoesNotExist()
    {
        // Arrange
        var admin = CreateAuthenticatedAdminClient();
        const string email = "missing@test.com";

        // Act
        var response = await admin.GetUserByEmailAsync(email, CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        await response.ContainsErrorCode("User.NotFound", CurrentCancellationToken);
    }

    [Fact]
    public async Task GetUserByEmail_Should_ReturnUser_When_Exists()
    {
        // Arrange
        var admin = CreateAuthenticatedAdminClient();

        var user = CreateUser("exists@test.com");

        await using var dbContext = GetDbContext();
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        // Act
        var response = await admin.GetUserByEmailAsync("exists@test.com", CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.MapTo<User>(CurrentCancellationToken);

        result.Email.Should().Be(user.Email);
    }


    [Fact]
    public async Task GetUserByEmail_Should_Return400_When_EmailIsInvalid()
    {
        // Arrange
        var admin = CreateAuthenticatedAdminClient();
        const string email = "not-an-email";

        // Act
        var response = await admin.GetUserByEmailAsync(email, CurrentCancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}