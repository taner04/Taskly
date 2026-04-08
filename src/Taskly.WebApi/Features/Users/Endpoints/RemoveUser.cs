using Taskly.WebApi.Common.Infrastructure.Persistence;
using Taskly.WebApi.Features.Users.Model;

namespace Taskly.WebApi.Features.Users.Endpoints;

[Handler]
[MapDelete(ApiRoutes.Users.RemoveUser)]
[Authorize(Policy = Policies.Admin)]
public static partial class RemoveUser
{
    internal static void CustomizeEndpoint(
        IEndpointConventionBuilder endpoint)
    {
        endpoint.WithTags(nameof(User));
    }

    private static async ValueTask HandleAsync(
        [AsParameters] Command command,
        TasklyDbContext context,
        CancellationToken ct)
    {
        var user = await context.Users.SingleOrDefaultAsync(u => u.Id == command.UserId, ct);

        if (user is null)
        {
            throw new ModelNotFoundException<User>(command.UserId.Value);
        }

        context.Users.Remove(user);
        await context.SaveChangesAsync(ct);
    }

    [Validate]
    public sealed partial record Command : IValidationTarget<Command>
    {
        public required UserId UserId { get; init; }
    }
}
