using Taskly.WebApi.Common.Infrastructure.Persistence;
using Taskly.WebApi.Features.Users.Model;

namespace Taskly.WebApi.Features.Users.Endpoints;

[Handler]
[MapGet(ApiRoutes.Users.GetUsers)]
[Authorize(Policy = Policies.Admin)]
public static partial class GetUsers
{
    internal static void CustomizeEndpoint(
        IEndpointConventionBuilder endpoint)
    {
        endpoint.WithTags(nameof(User));
    }

    private static async ValueTask<List<User>> HandleAsync(
        Query _,
        TasklyDbContext context,
        CancellationToken ct) =>
        await context.Users.ToListAsync(ct);

    public sealed record Query;
}
