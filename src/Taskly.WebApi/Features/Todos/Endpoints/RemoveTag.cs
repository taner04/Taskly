using Taskly.WebApi.Features.Todos.Common.Extensions;
using Taskly.WebApi.Features.Todos.Common.Specifications;
using TagId = Taskly.WebApi.Features.Tags.Common.Models.TagId;

namespace Taskly.WebApi.Features.Todos.Endpoints;

[Handler]
[MapDelete(ApiRoutes.Todos.RemoveTag)]
[Authorize(Policy = Security.Policies.User)]
public static partial class RemoveTag
{
    internal static void CustomizeEndpoint(
        RouteHandlerBuilder endpoint)
    {
        endpoint.WithTags(nameof(Todo));
        endpoint.RequireRateLimiting(Security.RateLimiting.Global);
    }

    internal static Ok<GetTodoResponse> TransformResult(
        GetTodoResponse response) =>
        TypedResults.Ok(response);

    private static async ValueTask<GetTodoResponse> HandleAsync(
        [AsParameters] Command command,
        TasklyDbContext context,
        CurrentUserService currentUserService,
        CancellationToken ct)
    {
        var userId = currentUserService.GetUserId();

        var spec = new TodoByUserIdSpecification(command.TodoId, userId);
        var todo = await context.Todos
            .WithSpecification(spec)
            .SingleOrDefaultAsync(ct) ?? throw new ModelNotFoundException<Todo>(command.TodoId.Value);

        var tagToRemove = todo.Tags.SingleOrDefault(t => t.Id == command.TagId) ??
                          throw new ModelNotFoundException<Tag>(command.TagId.Value);

        todo.Tags.Remove(tagToRemove);
        await context.SaveChangesAsync(ct);

        return todo.ToGetTodoResponse();
    }

    [Validate]
    public sealed partial record Command : IValidationTarget<Command>
    {
        [FromRoute] [NotEmpty] public required TodoId TodoId { get; init; }
        [FromRoute] [NotEmpty] public required TagId TagId { get; init; }
    }
}