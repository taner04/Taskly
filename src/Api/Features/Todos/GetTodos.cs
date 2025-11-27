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
        Query _,
        ApplicationDbContext context,
        CurrentUserService currentUserService,
        CancellationToken ct)
    {
        var userId = currentUserService.GetCurrentUserId();
        return await context.Todos.Where(t => t.UserId == userId).ToListAsync(ct);
    }

    public sealed record Query;
}