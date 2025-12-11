using Api.Features.Tags.Exceptions;

namespace Api.Features.Todos.Endpoints;

[Handler]
[MapPost(Routes.Todos.AddTags)]
[Authorize]
public static partial class AddTags
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

        var tags = await context.Tags
            .Where(tag => command.Body.TagIds.Contains(tag.Id) && tag.UserId == userId)
            .ToListAsync(ct);

        if (tags.Count == 0)
        {
            throw new TagsNotFoundExceptions(command.Body.TagIds);
        }

        var existingTagIds = todo.Tags.Select(t => t.Id).ToList();
        foreach (var tag in tags.Where(tag => !existingTagIds.Contains(tag.Id)))
        {
            todo.Tags.Add(tag);
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
            [NotEmpty] public required List<TagId> TagIds { get; init; }
        }
    }
}