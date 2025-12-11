using Api.Features.Todos.Specifications;
using Ardalis.Specification.EntityFrameworkCore;

namespace Api.Features.Todos.Endpoints;

[Handler]
[MapPost(Routes.Todos.Complete)]
[Authorize(Policy = Policies.User)]
public static partial class CompleteTodo
{
    internal static void CustomizeEndpoint(
        IEndpointConventionBuilder endpoint)
    {
        endpoint.WithTags(nameof(Todo));
    }

    private static async ValueTask HandleAsync(
        [AsParameters] Command command,
        ApplicationDbContext context,
        CurrentUserService currentUserService,
        CancellationToken ct)
    {
        var userId = currentUserService.GetUserId();
        var spec = new TodoByUserIdSpecification(command.TodoId, userId);
        var todo = await context.Todos
            .WithSpecification(spec)
            .SingleOrDefaultAsync(ct);

        if (todo is null)
        {
            throw new ModelNotFoundException<Todo>(command.TodoId.Value);
        }

        todo.SetCompletionStatus(command.Body.Completed);

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