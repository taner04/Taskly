using Microsoft.AspNetCore.Http.HttpResults;
using Taskly.Shared.Pagination;
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

    internal static Ok<PaginationResult<Response>> TransformResult(
        PaginationResult<Response> result) =>
        TypedResults.Ok(result);

    private static async ValueTask<PaginationResult<Response>> HandleAsync(
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


    public sealed record Response(Guid Id, string Name);
}

public sealed class GetTagsMapper : IPaginationMapper<Tag, GetTags.Response>
{
    public List<GetTags.Response> Map(List<Tag> source)
    {
        return source.Select(t => new GetTags.Response(
            t.Id.Value,
            t.Name
        )).ToList();
    }
}