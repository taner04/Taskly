namespace Api.Features.Todos.Endpoints;

[Handler]
[MapDelete(Routes.Todos.Remove)]
[Authorize]
public static partial class RemoveTodo
{
    internal static void CustomizeEndpoint(
        IEndpointConventionBuilder endpoint)
    {
        endpoint.WithTags(nameof(Todo));
    }

    private static async ValueTask HandleAsync(
        Command command,
        ApplicationDbContext context,
        CurrentUserService currentUserService,
        CancellationToken ct)
    {
        var userId = currentUserService.GetUserId();
        var todo = await context.Todos.SingleOrDefaultAsync(
            t => t.Id == command.TodoId && t.UserId == userId, ct);

        if (todo is null)
        {
            throw new ModelNotFoundException<Todo>(command.TodoId.Value);
        }

        context.Todos.Remove(todo);
        await context.SaveChangesAsync(ct);
    }

    [Validate]
    public sealed partial record Command : IValidationTarget<Command>
    {
        [FromRoute] [NotEmpty] public required TodoId TodoId { get; init; }
    }
}