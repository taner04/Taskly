using UserId = Taskly.WebApi.Features.Users.Models.UserId;

namespace Taskly.WebApi.Features.Users.Endpoints;

[Handler]
[MapDelete(ApiRoutes.Users.RemoveUser)]
[Authorize(Policy = Policies.Roles.Admin)]
public static partial class RemoveUser
{
    internal static void CustomizeEndpoint(
        RouteHandlerBuilder endpoint)
    {
        endpoint.WithTags(nameof(User));
        endpoint.RequireRateLimiting(Policies.RateLimiting.Global);
    }

    private static async ValueTask HandleAsync(
        [AsParameters] Command command,
        TasklyDbContext context,
        CancellationToken ct)
    {
        var user = await context.Users.SingleOrDefaultAsync(u => u.Id == command.UserId, ct) ??
                   throw new ModelNotFoundException<User>(command.UserId.Value);

        context.Users.Remove(user);
        await context.SaveChangesAsync(ct);
    }

    [Validate]
    public sealed partial record Command : IValidationTarget<Command>
    {
        public required UserId UserId { get; init; }
    }
}