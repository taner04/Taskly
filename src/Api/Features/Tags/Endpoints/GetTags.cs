using Api.Features.Shared.Dtos.Tags;

namespace Api.Features.Tags.Endpoints;

[Handler]
[MapGet(Routes.Tags.GetTags)]
[Authorize]
public static partial class GetTags
{
    internal static void CustomizeEndpoint(
        IEndpointConventionBuilder endpoint)
    {
        endpoint.WithTags(nameof(Tag));
    }

    private static async ValueTask<List<TagDto>> HandleAsync(
        Query _,
        ApplicationDbContext context,
        CurrentUserService currentUserService,
        CancellationToken ct)
    {
        var userId = currentUserService.GetUserId();
        var tags = await context.Tags.Where(t => t.UserId == userId).ToListAsync(ct);

        return tags.Select(TagDto.FromDomain).ToList();
    }

    public sealed record Query;
}