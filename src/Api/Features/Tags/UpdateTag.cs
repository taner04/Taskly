using Api.Features.Tags.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.Tags;

[Handler]
[MapPut(ApiRoutes.Tags.Update)]
[Authorize]
public static partial class UpdateTag
{
    internal static void CustomizeEndpoint(IEndpointConventionBuilder endpoint)
    {
        endpoint.WithTags(nameof(Todo));
    }

    internal static Results<Ok, NotFound<Error>, BadRequest<Error>> TransformResult(ErrorOr<Success> result)
    {
        if (result.FirstError.Type == ErrorType.NotFound)
        {
            return result.Match<Results<Ok, NotFound<Error>, BadRequest<Error>>>(
                _ => TypedResults.Ok(),
                error => TypedResults.NotFound(error.First()));
        }

        return result.Match<Results<Ok, NotFound<Error>, BadRequest<Error>>>(
            _ => TypedResults.Ok(),
            error => TypedResults.BadRequest(error.First()));
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

        var renameResult = tag.Rename(command.Body.NewName);
        if (renameResult.IsError)
        {
            return renameResult.Errors;
        }

        context.Tags.Update(tag);
        await context.SaveChangesAsync(ct);

        return Result.Success;
    }

    [Validate]
    public sealed partial record Command : IValidationTarget<Command>
    {
        [FromRoute] [NotEmpty] public TagId TagId { get; init; }
        [NotNull] public CommandBody Body { get; init; } = null!;

        [Validate]
        public sealed partial record CommandBody : IValidationTarget<CommandBody>
        {
            [NotEmpty] [NotNull] public required string NewName { get; init; }
        }
    }
}