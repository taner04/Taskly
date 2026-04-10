namespace Taskly.IntegrationTests.Factories;

internal static class UserFactory
{
    internal const string Email = "userfactory@mail.com";
    internal const string Sub = "auth0|userfactory123";

    internal static User Create() => User.Create(Email, Sub);
}