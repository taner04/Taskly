using Api.Features.Shared.Api;
using Api.Features.Shared.Dtos.Tags;
using Api.Features.Tags.Model;
using Api.Features.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Features.Tags;

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

    internal static Ok<List<TagDto>> TransformResult(
        List<TagDto> result)
    {
        return TypedResults.Ok(result);
    }

    private static async ValueTask<List<TagDto>> HandleAsync(
        Query _,
        ApplicationDbContext context,
        CurrentUserService currentUserService,
        CancellationToken ct)
    {
        var userId = currentUserService.GetCurrentUserId();
        var tags = await context.Tags.Where(t => t.UserId == userId).ToListAsync(ct);

        return tags.Select(TagDto.FromDomain).ToList();
    }

    public sealed record Query;
}