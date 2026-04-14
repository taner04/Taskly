namespace Taskly.WebApi.IntegrationTests.Factories;

internal static class TagFactory
{
    internal static Tag Create(string name, UserId userId) =>
        Tag.Create(name, userId);
}