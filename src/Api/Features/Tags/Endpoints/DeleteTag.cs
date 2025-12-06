namespace Api.Features.Tags.Endpoints;

[Handler]
[MapDelete(Routes.Tags.Delete)]
[Authorize]
public static partial class DeleteTag
{
    internal static void CustomizeEndpoint(
        IEndpointConventionBuilder endpoint)
    {
        endpoint.WithTags(nameof(Tag));
    }

    private static async ValueTask HandleAsync(
        [AsParameters] Command command,
        ApplicationDbContext context,
        CurrentUserService currentUserService,
        CancellationToken ct)
    {
        var userId = currentUserService.GetCurrentUserId();
        var tag = await context.Tags
            .SingleOrDefaultAsync(t => t.Id == command.TagId && t.UserId == userId, ct);

        if (tag is null)
        {
            throw new ModelNotFoundException<Tag>(command.TagId.Value);
        }

        var todos = await context.Todos
            .Where(todo =>
                todo.UserId == userId &&
                todo.Tags.Any(t => t.Id == command.TagId))
            .Include(todo => todo.Tags)
            .ToListAsync(ct);

        todos.ForEach(t => t.Tags.Remove(tag));

        context.Tags.Remove(tag);
        await context.SaveChangesAsync(ct);
    }

    [Validate]
    public sealed partial record Command : IValidationTarget<Command>
    {
        public required TagId TagId { get; init; }
    }
}