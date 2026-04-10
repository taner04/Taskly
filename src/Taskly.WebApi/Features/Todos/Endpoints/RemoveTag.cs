using Taskly.WebApi.Features.Todos.Specifications;
using TagId = Taskly.WebApi.Features.Tags.Models.TagId;

namespace Taskly.WebApi.Features.Todos.Endpoints;

[Handler]
[MapDelete(ApiRoutes.Todos.RemoveTag)]
[Authorize(Policy = Policies.Roles.User)]
public static partial class RemoveTag
{
    internal static void CustomizeEndpoint(
        IEndpointConventionBuilder endpoint)
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

        var tagToRemove = todo.Tags.SingleOrDefault(t => t.Id == command.TagId) ??
                          throw new ModelNotFoundException<Tag>(command.TagId.Value);

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