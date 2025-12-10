namespace Api.Features.Todos.Endpoints;

[Handler]
[MapPut(Routes.Todos.Update)]
[Authorize]
public static partial class UpdateTodo
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
        var todo = await context.Todos.SingleOrDefaultAsync(
            t => t.Id == command.TodoId && t.UserId == userId, ct);

        if (todo is null)
        {
            throw new ModelNotFoundException<Todo>(command.TodoId.Value);
        }

        todo.Update(command.Body.Title, command.Body.Description, command.Body.Priority);

        context.Todos.Update(todo);
        await context.SaveChangesAsync(ct);
    }

    [Validate]
    public sealed partial record Command : IValidationTarget<Command>
    {
        [FromRoute] [NotEmpty] public required TodoId TodoId { get; init; }
        [NotNull] public CommandBody Body { get; init; } = null!;

        [Validate]
        public sealed partial record CommandBody : IValidationTarget<CommandBody>
        {
            [NotEmpty] public required string Title { get; init; }
            public string? Description { get; init; }
            public required TodoPriority Priority { get; init; }
        }
    }
}