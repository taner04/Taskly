using Taskly.WebApi.Common.Shared;
using Taskly.WebApi.Common.Shared.Pagination;
using Taskly.WebApi.Features.Users.Models;

namespace Taskly.WebApi.Features.Users.Endpoints;

[Handler]
[MapGet(ApiRoutes.Users.GetUsers)]
[Authorize(Policy = Policies.Roles.Admin)]
public static partial class GetUsers
{
    internal static void CustomizeEndpoint(
        IEndpointConventionBuilder endpoint)
    {
        endpoint.WithTags(nameof(User));
        endpoint.RequireRateLimiting(Policies.RateLimiting.Global);
    }

    private static async ValueTask<PaginationResult<User>> HandleAsync(
        Query query,
        PaginationService paginationService,
        CancellationToken ct) => await paginationService.GetPaginationResultAsync<User>(query, ct);

    public sealed record Query(int PageIndex, int PageSize) : PaginationQuery(PageIndex, PageSize);
}