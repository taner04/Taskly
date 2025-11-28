using Api.Features.Tags.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Features.Tags;

[Handler]
[MapGet(ApiRoutes.Tags.GetTags)]
[Authorize]
public static partial class GetTags
{
    internal static void CustomizeEndpoint(IEndpointConventionBuilder endpoint)
    {
        endpoint.WithTags(nameof(Tag));
    }

    internal static Ok<List<Dto>> TransformResult(List<Dto> result)
    {
        return TypedResults.Ok(result);
    }

    private static async ValueTask<List<Dto>> HandleAsync(
        Query _,
        ApplicationDbContext context,
        CurrentUserService currentUserService,
        CancellationToken ct)
    {
        var userId = currentUserService.GetCurrentUserId();
        var tags = await context.Tags.Where(t => t.UserId == userId).ToListAsync(ct);

        return tags.Select(Dto.FromDomain).ToList();
    }

    public sealed record Query;

    public sealed record Dto(
        Guid Id,
        string Name,
        string UserId
    )
    {
        public static Dto FromDomain(Tag tag)
        {
            return new Dto(
                tag.Id.Value,
                tag.Name,
                tag.UserId
            );
        }
    }
}