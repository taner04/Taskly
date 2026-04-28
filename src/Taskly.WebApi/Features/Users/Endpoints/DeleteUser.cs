using Taskly.WebApi.Features.Users.Common.Models;
using UserId = Taskly.WebApi.Features.Users.Common.Models.UserId;

namespace Taskly.WebApi.Features.Users.Endpoints;

[Handler]
[MapDelete(ApiRoutes.Users.RemoveUser)]
[Authorize(Policy = Security.Policies.Admin)]
public static partial class DeleteUser
{
    internal static void CustomizeEndpoint(
        RouteHandlerBuilder endpoint)
    {
        endpoint.WithTags(nameof(User));
        endpoint.RequireRateLimiting(Security.RateLimiting.Global);
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