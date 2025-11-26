using Microsoft.AspNetCore.Authorization;

namespace Api.Features.Todos;

[Handler]
[MapGet(ApiRoutes.Todos.GetAll)]
[Authorize]
public static partial class GetTodos
{
    internal static void CustomizeEndpoint(IEndpointConventionBuilder endpoint)
    {
        endpoint.WithTags(nameof(Todo));
    }

    private static async ValueTask<ErrorOr<IEnumerable<Todo>>> HandleAsync(
        Query query, 
        ApplicationDbContext context,
        CancellationToken ct)
    {
        return await context.Todos.Where(t => t.UserId == query.UserId).ToListAsync(ct);
    }
    
    public sealed record Query : UserRequest;
}