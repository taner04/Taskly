using Taskly.WebApi.Features.Todos.Specifications;

namespace Taskly.WebApi.Features.Todos.Endpoints;

[Handler]
[MapPost(ApiRoutes.Todos.Complete)]
[Authorize(Policy = Policies.Roles.User)]
public static partial class CompleteTodo
{
    internal static void CustomizeEndpoint(
        RouteHandlerBuilder endpoint)
    {
        endpoint.WithTags(nameof(Todo));
        endpoint.RequireRateLimiting(Policies.RateLimiting.Global);
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

        if (command.Body.Completed != todo.IsCompleted)
        {
            todo.IsCompleted = command.Body.Completed;
        }

        context.Todos.Update(todo);
        await context.SaveChangesAsync(ct);
    }

    [Validate]
    public sealed partial record Command : IValidationTarget<Command>
    {
        [FromRoute] [NotEmpty] public required TodoId TodoId { get; init; }
        [NotNull] public required CommandBody Body { get; init; } = null!;

        [Validate]
        public sealed partial record CommandBody : IValidationTarget<CommandBody>
        {
            public required bool Completed { get; init; }
        }
    }
}