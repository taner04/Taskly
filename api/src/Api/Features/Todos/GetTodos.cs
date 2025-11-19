namespace Api.Features.Todos;

[Handler]
[MapGet("api/todos")]
public static partial class GetTodos
{
    internal static void CustomizeEndpoint(IEndpointConventionBuilder endpoint)
    {
        endpoint.WithTags(nameof(Todo));
    }

    private static async ValueTask<ErrorOr<IEnumerable<Todo>>> HandleAsync(Query query, ApplicationDbContext context,
        CancellationToken ct)
    {
        return await context.Todos.Where(t => t.UserId == query.UserId).ToListAsync(ct);
    }

    // ReSharper disable once ClassNeverInstantiated.Global
    public sealed record Query : UserRequest;
}