namespace Taskly.WebApi.Features.Todos.Endpoints;

[Handler]
[MapPost(ApiRoutes.Todos.Create)]
[Authorize(Policy = Security.Policies.User)]
public static partial class CreateTodo
{
    internal static void CustomizeEndpoint(
        RouteHandlerBuilder endpoint)
    {
        endpoint.WithTags(nameof(Todo));
        endpoint.RequireRateLimiting(Security.RateLimiting.Global);
    }

    private static async ValueTask HandleAsync(
        [FromBody] Command command,
        TasklyDbContext context,
        CurrentUserService currentUserService,
        CancellationToken ct)
    {
        var userId = currentUserService.GetUserId();

        var newTodo = Todo.Create(command.Title, command.Description, command.Priority, userId);

        await context.Todos.AddAsync(newTodo, ct);
        await context.SaveChangesAsync(ct);
    }

    [Validate]
    public sealed partial record Command : IValidationTarget<Command>
    {
        [NotEmpty] public required string Title { get; init; }
        public string? Description { get; init; }
        public required TodoPriority Priority { get; init; }
    }
}