namespace IntegrationTests.Common;

public static class RouteExtensions
{
    public static string WithId(this string route, Guid id)
    {
        return route.Replace("{todoId:guid}", id.ToString());
    }
}