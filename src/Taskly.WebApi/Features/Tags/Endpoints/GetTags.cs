using Taskly.WebApi.Common.Infrastructure.Persistence;
using Taskly.WebApi.Features.Shared.Dtos.Tags;

namespace Taskly.WebApi.Features.Tags.Endpoints;

[Handler]
[MapGet(ApiRoutes.Tags.GetTags)]
[Authorize(Policy = Policies.User)]
public static partial class GetTags
{
    internal static void CustomizeEndpoint(
        IEndpointConventionBuilder endpoint)
    {
        endpoint.WithTags(nameof(Tag));
    }

    private static async ValueTask<List<TagDto>> HandleAsync(
        Query _,
        TasklyDbContext context,
        CurrentUserService currentUserService,
        CancellationToken ct)
    {
        var userId = currentUserService.GetUserId();
        var tags = await context.Tags.Where(t => t.UserId == userId).ToListAsync(ct);

        return tags.Select(TagDto.FromDomain).ToList();
    }

    public sealed record Query;
}
