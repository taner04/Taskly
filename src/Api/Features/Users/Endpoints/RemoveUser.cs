using Api.Features.Users.Model;

namespace Api.Features.Users.Endpoints;

[Handler]
[MapDelete(Routes.Users.RemoveUser)]
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
        ApplicationDbContext context,
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