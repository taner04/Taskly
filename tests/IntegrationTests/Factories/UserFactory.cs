using Api.Features.Users.Model;

namespace IntegrationTests.Factories;

internal static class UserFactory
{
    private const string DefaultEmail = "test@mail.com";
    
    internal static User Create(string auth0Id)
    {
        return User.Create(DefaultEmail, auth0Id);
    }
}