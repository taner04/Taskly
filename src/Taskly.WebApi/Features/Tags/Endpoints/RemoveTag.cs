using Taskly.WebApi.Features.Tags.Specifications;
using TagId = Taskly.WebApi.Features.Tags.Models.TagId;

namespace Taskly.WebApi.Features.Tags.Endpoints;

[Handler]
[MapDelete(ApiRoutes.Tags.Remove)]
[Authorize(Policy = Policies.Roles.User)]
public static partial class RemoveTag
{
    internal static void CustomizeEndpoint(
        RouteHandlerBuilder endpoint)
    {
        endpoint.WithTags(nameof(Tag));
        endpoint.RequireRateLimiting(Policies.RateLimiting.Global);
    }

    private static async ValueTask HandleAsync(
        [AsParameters] Command command,
        TasklyDbContext context,
        CurrentUserService currentUserService,
        CancellationToken ct)
    {
        var userId = currentUserService.GetUserId();

        var spec = new TagByIdSpecification(command.TagId, userId);
        var tag = await context.Tags
            .WithSpecification(spec)
            .SingleOrDefaultAsync(ct) ?? throw new ModelNotFoundException<Tag>(command.TagId.Value);

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