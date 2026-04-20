using Taskly.WebApi.Features.Todos.Exceptions;
using Taskly.WebApi.Features.Todos.Specifications;

namespace Taskly.WebApi.Features.Todos.Endpoints;

[Handler]
[MapPut(ApiRoutes.Todos.Update)]
[Authorize(Policy = Security.Policies.User)]
public static partial class UpdateTodo
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
        InvalidTodoTitleException.ThrowIfInvalid(command.Body.Title);
        InvalidTodoDescriptionException.ThrowIfInvalid(command.Body.Description);

        var userId = currentUserService.GetUserId();

        var spec = new TodoByUserIdSpecification(command.TodoId, userId);
        var todo = await context.Todos
            .WithSpecification(spec)
            .SingleOrDefaultAsync(ct) ?? throw new ModelNotFoundException<Todo>(command.TodoId.Value);

        todo.Title = command.Body.Title;
        todo.Description = command.Body.Description;
        todo.Priority = command.Body.Priority;

        context.Todos.Update(todo);
        await context.SaveChangesAsync(ct);
    }

    [Validate]
    public sealed partial record Command : IValidationTarget<Command>
    {
        [FromRoute] [NotEmpty] public required TodoId TodoId { get; init; }
        [FromBody] [NotNull] public required CommandBody Body { get; init; } = null!;

        [Validate]
        public sealed partial record CommandBody : IValidationTarget<CommandBody>
        {
            [NotEmpty] public required string Title { get; init; }
            public string? Description { get; init; }
            public required TodoPriority Priority { get; init; }
        }
    }
}