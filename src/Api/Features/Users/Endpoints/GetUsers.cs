using Api.Features.Users.Model;

namespace Api.Features.Users.Endpoints;

[Handler]
[MapGet(Routes.Users.GetUsers)]
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
        ApplicationDbContext context,
        CancellationToken ct)
    {
        return await context.Users.ToListAsync(ct);
    }

    public sealed record Query;
}