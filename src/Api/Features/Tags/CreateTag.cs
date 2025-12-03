using Api.Features.Tags.Model;
using Api.Features.Users;
using Api.Shared.Features.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Features.Tags;

[Handler]
[MapPost(Routes.Tags.Create)]
[Authorize]
public static partial class CreateTag
{
    internal static void CustomizeEndpoint(
        IEndpointConventionBuilder endpoint)
    {
        endpoint.WithTags(nameof(Tag));
    }

    internal static Results<Ok, BadRequest<Error>> TransformResult(
        ErrorOr<Success> result)
    {
        return result.Match<Results<Ok, BadRequest<Error>>>(
            _ => TypedResults.Ok(),
            error => TypedResults.BadRequest(error.First()));
    }

    private static async ValueTask<ErrorOr<Success>> HandleAsync(
        Command command,
        ApplicationDbContext context,
        CurrentUserService currentUserService,
        CancellationToken ct)
    {
        var userId = currentUserService.GetCurrentUserId();
        var tagOrError = Tag.TryCreate(command.TagName, userId);
        if (tagOrError.IsError)
        {
            return tagOrError.Errors;
        }

        var newTag = tagOrError.Value;

        if (await context.Tags.AnyAsync(t => t.Name == newTag.Name && t.UserId == userId, ct))
        {
            return Error.Conflict("Tag.Exists", "A tag with the same name already exists.");
        }

        context.Tags.Add(newTag);
        await context.SaveChangesAsync(ct);

        return Result.Success;
    }

    [Validate]
    public sealed partial record Command : IValidationTarget<Command>, ITransactionalRequest
    {
        [NotEmpty] [NotNull] public required string TagName { get; init; }
    }
}