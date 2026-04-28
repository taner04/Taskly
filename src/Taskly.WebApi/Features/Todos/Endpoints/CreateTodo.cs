using Taskly.WebApi.Features.Todos.Common.Extensions;

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

    internal static Created<GetTodoResponse> TransformResult(
        GetTodoResponse result) =>
        TypedResults.Created(ApiRoutes.Todos.Create, result);

    private static async ValueTask<GetTodoResponse> HandleAsync(
        [FromBody] Command command,
        TasklyDbContext context,
        CurrentUserService currentUserService,
        CancellationToken ct)
    {
        var userId = currentUserService.GetUserId();

        var todo = Todo.Create(command.Title, command.Description, command.Priority, userId);

        await context.Todos.AddAsync(todo, ct);
        await context.SaveChangesAsync(ct);

        return todo.ToGetTodoResponse();
    }

    [Validate]
    public sealed partial record Command : IValidationTarget<Command>
    {
        [NotEmpty] public required string Title { get; init; }
        public string? Description { get; init; }
        public required TodoPriority Priority { get; init; }
    }
}