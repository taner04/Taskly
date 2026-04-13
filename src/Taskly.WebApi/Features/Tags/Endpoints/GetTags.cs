using Taskly.WebApi.Common.Shared.Pagination;

namespace Taskly.WebApi.Features.Tags.Endpoints;

[Handler]
[MapGet(ApiRoutes.Tags.GetTags)]
[Authorize(Policy = Policies.Roles.User)]
public static partial class GetTags
{
    internal static void CustomizeEndpoint(
        RouteHandlerBuilder endpoint)
    {
        endpoint.WithTags(nameof(Tag));
        endpoint.RequireRateLimiting(Policies.RateLimiting.Global);
    }

    private static async ValueTask<PaginationResult<TagDto>> HandleAsync(
        Query query,
        CurrentUserService currentUserService,
        PaginationService paginationService,
        CancellationToken ct)
    {
        var userId = currentUserService.GetUserId();

        return await paginationService.GetPaginationResultAsync(
            query,
            new GetTagsMapper(),
            q => q.Where(t => t.UserId == userId),
            ct);
    }

    public sealed record Query(int PageIndex, int PageSize) : PaginationQuery(PageIndex, PageSize);
}

public sealed class GetTagsMapper : IPaginationMapper<Tag, TagDto>
{
    public List<TagDto> Map(List<Tag> source) => source.Select(TagDto.FromDomain).ToList();
}