using Taskly.WebApi.Features.Tags.Exceptions;
using Taskly.WebApi.Features.Todos.Specifications;
using TagId = Taskly.WebApi.Features.Tags.Models.TagId;

namespace Taskly.WebApi.Features.Todos.Endpoints;

[Handler]
[MapPost(ApiRoutes.Todos.AddTags)]
[Authorize(Policy = Policies.Roles.User)]
public static partial class AddTags
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

        var spec = new TodoByUserIdWithTagsSpecification(command.TodoId, userId);
        var todo = await context.Todos
            .WithSpecification(spec)
            .SingleOrDefaultAsync(ct) ?? throw new ModelNotFoundException<Todo>(command.TodoId.Value);

        var tags = await context.Tags
            .Where(tag => command.Body.TagIds.Contains(tag.Id) && tag.UserId == userId)
            .ToListAsync(ct);

        if (tags.Count == 0)
        {
            throw new TagNotFoundException(command.Body.TagIds);
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