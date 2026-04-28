using Taskly.Shared.Pagination;
using Taskly.Shared.WebApi.Responses.Users;
using Taskly.WebApi.Common.Shared.Pagination;
using Taskly.WebApi.Features.Users.Common.Mapper;
using Taskly.WebApi.Features.Users.Common.Models;

namespace Taskly.WebApi.Features.Users.Endpoints;

[Handler]
[MapGet(ApiRoutes.Users.GetUsers)]
[Authorize(Policy = Security.Policies.Admin)]
public static partial class GetUsers
{
    internal static void CustomizeEndpoint(
        RouteHandlerBuilder endpoint)
    {
        endpoint.WithTags(nameof(User));
        endpoint.RequireRateLimiting(Security.RateLimiting.Global);
    }

    private static async ValueTask<PaginationResult<GetUserResponse>> HandleAsync(
        PaginationQuery query,
        PaginationService paginationService,
        CancellationToken ct) =>
        await paginationService.GetPaginationResultAsync(query, new UserMapper(), ct);
}