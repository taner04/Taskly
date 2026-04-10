using Microsoft.AspNetCore.Http.HttpResults;
using Taskly.WebApi.Features.Tags.Exceptions;

namespace Taskly.WebApi.Features.Tags.Endpoints;

[Handler]
[MapPost(ApiRoutes.Tags.Create)]
[Authorize(Policy = Policies.Roles.User)]
public static partial class CreateTag
{
    internal static void CustomizeEndpoint(
        IEndpointConventionBuilder endpoint)
    {
        endpoint.WithTags(nameof(Tag));
        endpoint.RequireRateLimiting(Policies.RateLimiting.Global);
    }

    internal static Created<Response> TransformResult(
        Response response) =>
        TypedResults.Created($"api/todos/{response.TagId}", response);

    private static async ValueTask<Response> HandleAsync(
        Command command,
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

        return new Response(newTag.Id.Value);
    }

    [Validate]
    public sealed partial record Command : IValidationTarget<Command>
    {
        [NotEmpty] public required string TagName { get; init; }
    }

    public sealed record Response(Guid TagId);
}