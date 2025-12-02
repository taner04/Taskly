namespace IntegrationTests.Extensions;

public static class RouteExtensions
{
    public static string ParseTodoRoute(
        this string route,
        Guid id)
    {
        return route.ReplaceRouteId("{todoId:guid}", id.ToString());
    }

    public static string ParseTagRoute(
        this string route,
        Guid id)
    {
        return route.ReplaceRouteId("{tagId:guid}", id.ToString());
    }

    public static string ReplaceRouteId(
        this string route,
        string placeholder,
        string value)
    {
        return route.Replace(placeholder, value);
    }
}