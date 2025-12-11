namespace Api.Features.Todos.Endpoints;

[Handler]
[MapDelete(Routes.Todos.RemoveTag)]
[Authorize]
public static partial class RemoveTag
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
        var todo = await context.Todos
            .Include(t => t.Tags)
            .SingleOrDefaultAsync(t => t.Id == command.TodoId && t.UserId == userId, ct);

        if (todo is null)
        {
            throw new ModelNotFoundException<Todo>(command.TodoId.Value);
        }

        var tagToRemove = todo.Tags.SingleOrDefault(t => t.Id == command.TagId);

        if (tagToRemove is null)
        {
            throw new ModelNotFoundException<Tag>(command.TagId.Value);
        }

        todo.Tags.Remove(tagToRemove);
        await context.SaveChangesAsync(ct);
    }

    [Validate]
    public sealed partial record Command : IValidationTarget<Command>
    {
        [FromRoute] [NotEmpty] public required TodoId TodoId { get; init; }
        [FromRoute] [NotEmpty] public required TagId TagId { get; init; }
    }
}