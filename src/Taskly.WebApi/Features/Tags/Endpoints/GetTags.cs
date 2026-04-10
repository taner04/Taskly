using Taskly.Shared.Attributes;
using Taskly.WebApi.Common.Shared;
using Taskly.WebApi.Common.Shared.Pagination;
using Taskly.WebApi.Features.Tags.Models;

namespace Taskly.WebApi.Features.Tags.Endpoints;

[Handler]
[MapGet(ApiRoutes.Tags.GetTags)]
[Authorize(Policy = Policies.Roles.User)]
public static partial class GetTags
{
    internal static void CustomizeEndpoint(
        IEndpointConventionBuilder endpoint)
    {
        endpoint.WithTags(nameof(Tag));
        endpoint.RequireRateLimiting(Policies.RateLimiting.Global);
    }

    private static async ValueTask<PaginationResult<TagDto>> HandleAsync(
        Query query,
        CurrentUserService currentUserService,
        PaginationService paginationService,
        GetTagsMapper mapper,
        CancellationToken ct)
    {
        var userId = currentUserService.GetUserId();

        return await paginationService.GetPaginationResultAsync(
            query,
            mapper,
            q => q.Where(t => t.UserId == userId),
            ct);
    }

    public sealed record Query(int PageIndex, int PageSize) : PaginationQuery(PageIndex, PageSize);
}

[ServiceInjection(ServiceLifetime.Singleton)]
public sealed class GetTagsMapper : IPaginationMapper<Tag, TagDto>
{
    public List<TagDto> Map(List<Tag> source) => source.Select(TagDto.FromDomain).ToList();
}