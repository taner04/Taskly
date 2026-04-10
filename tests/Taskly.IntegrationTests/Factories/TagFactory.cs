namespace Taskly.IntegrationTests.Factories;

internal static class TagFactory
{
    internal static Tag Create(string name, UserId userId) =>
        Tag.Create(name, userId);
}