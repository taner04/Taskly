using Taskly.Shared.Pagination;
using Taskly.Shared.WebApi.Responses.Tags;
using Taskly.WebApi.Common.Shared.Pagination;

namespace Taskly.WebApi.Features.Tags.Endpoints;

[Handler]
[MapGet(ApiRoutes.Tags.GetTags)]
[Authorize(Policy = Security.Policies.User)]
public static partial class GetTags
{
    internal static void CustomizeEndpoint(
        RouteHandlerBuilder endpoint)
    {
        endpoint.WithTags(nameof(Tag));
        endpoint.RequireRateLimiting(Security.RateLimiting.Global);
    }

    internal static Ok<PaginationResult<GetTagResponse>> TransformResult(
        PaginationResult<GetTagResponse> result) =>
        TypedResults.Ok(result);

    private static async ValueTask<PaginationResult<GetTagResponse>> HandleAsync(
        PaginationQuery query,
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
}

public sealed class GetTagsMapper : IMapper<Tag, GetTagResponse>
{
    public IEnumerable<GetTagResponse> Map(IEnumerable<Tag> source)
    {
        return source.Select(t => new GetTagResponse(
            t.Id.Value,
            t.Name
        )).ToList();
    }
}