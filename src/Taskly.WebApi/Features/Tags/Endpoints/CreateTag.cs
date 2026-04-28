using Taskly.Shared.WebApi.Responses.Tags;
using Taskly.WebApi.Features.Tags.Common.Exceptions;

namespace Taskly.WebApi.Features.Tags.Endpoints;

[Handler]
[MapPost(ApiRoutes.Tags.Create)]
[Authorize(Policy = Security.Policies.User)]
public static partial class CreateTag
{
    internal static void CustomizeEndpoint(
        RouteHandlerBuilder endpoint)
    {
        endpoint.WithTags(nameof(Tag));
        endpoint.RequireRateLimiting(Security.RateLimiting.Global);
    }

    internal static Created<GetTagResponse> TransformResult(
        GetTagResponse result) =>
        TypedResults.Created(ApiRoutes.Tags.Create, result);

    private static async ValueTask<GetTagResponse> HandleAsync(
        [FromBody] Command command,
        TasklyDbContext context,
        CurrentUserService currentUserService,
        CancellationToken ct)
    {
        var userId = currentUserService.GetUserId();

        if (await context.Tags.AnyAsync(t => t.Name == command.TagName && t.UserId == userId, ct))
        {
            throw new DuplicateTagException(command.TagName);
        }

        var newTag = Tag.Create(command.TagName, userId);
        context.Tags.Add(newTag);
        await context.SaveChangesAsync(ct);

        return new GetTagResponse(newTag.Id.Value, newTag.Name);
    }

    [Validate]
    public sealed partial record Command : IValidationTarget<Command>
    {
        [NotEmpty] public required string TagName { get; init; }
    }
}