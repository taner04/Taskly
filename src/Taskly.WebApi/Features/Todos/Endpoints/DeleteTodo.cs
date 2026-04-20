using Taskly.WebApi.Features.Todos.Specifications;

namespace Taskly.WebApi.Features.Todos.Endpoints;

[Handler]
[MapDelete(ApiRoutes.Todos.Remove)]
[Authorize(Policy = Security.Policies.User)]
public static partial class DeleteTodo
{
    internal static void CustomizeEndpoint(
        RouteHandlerBuilder endpoint)
    {
        endpoint.WithTags(nameof(Todo));
        endpoint.RequireRateLimiting(Security.RateLimiting.Global);
    }

    private static async ValueTask HandleAsync(
        [AsParameters] Command command,
        TasklyDbContext context,
        CurrentUserService currentUserService,
        CancellationToken ct)
    {
        var userId = currentUserService.GetUserId();

        var spec = new TodoByUserIdSpecification(command.TodoId, userId);
        var todo = await context.Todos
            .WithSpecification(spec)
            .SingleOrDefaultAsync(ct) ?? throw new ModelNotFoundException<Todo>(command.TodoId.Value);

        context.Todos.Remove(todo);
        await context.SaveChangesAsync(ct);
    }

    [Validate]
    public sealed partial record Command : IValidationTarget<Command>
    {
        [FromRoute] [NotEmpty] public required TodoId TodoId { get; init; }
    }
}