using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;

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
    
    internal static Ok<List<Dto>> TransformResult(List<Dto> result)
    {
        return TypedResults.Ok(result);
    }

    private static async ValueTask<List<Dto>> HandleAsync(
        Query _,
        ApplicationDbContext context,
        CurrentUserService currentUserService,
        CancellationToken ct)
    {
        var userId = currentUserService.GetCurrentUserId();
        var todos = await context.Todos.Where(t => t.UserId == userId).ToListAsync(ct);
        
        return todos.Select(Dto.FromDomain).ToList();
    }

    public sealed record Query;
    
    public sealed record Dto(
        Guid Id,
        string Title,
        string? Description,
        TodoPriority Priority,
        bool IsCompleted,
        string UserId
    )
    {
        public static Dto FromDomain(Todo todo) =>
            new(
                todo.Id.Value,
                todo.Title,
                todo.Description,
                todo.Priority,
                todo.IsCompleted,
                todo.UserId
            );
    }
}