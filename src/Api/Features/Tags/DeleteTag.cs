using Api.Features.Tags.Model;
using Api.Features.Users;
using Api.Shared.Features.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.Tags;

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

    internal static Results<Ok, NotFound<Error>> TransformResult(
        ErrorOr<Success> result)
    {
        return result.Match<Results<Ok, NotFound<Error>>>(
            _ => TypedResults.Ok(),
            error => TypedResults.NotFound(error.First()));
    }

    private static async ValueTask<ErrorOr<Success>> HandleAsync(
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
            return Error.NotFound("Tag.NotFound", "The specified tag was not found.");
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

        return Result.Success;
    }

    [Validate]
    public sealed partial record Command : IValidationTarget<Command>, ITransactionalRequest
    {
        [FromRoute] [NotEmpty] public required TagId TagId { get; init; }
    }
}