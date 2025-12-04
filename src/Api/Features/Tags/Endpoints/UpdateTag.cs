using Api.Features.Tags.Exceptions;
using Api.Features.Tags.Model;
using Api.Features.Users.Services;
using Api.Shared.Api;
using Microsoft.AspNetCore.Authorization;

namespace Api.Features.Tags.Endpoints;

[Handler]
[MapPut(Routes.Tags.Update)]
[Authorize]
public static partial class UpdateTag
{
    internal static void CustomizeEndpoint(
        IEndpointConventionBuilder endpoint)
    {
        endpoint.WithTags(nameof(Tag));
    }

    private static async ValueTask HandleAsync(
        [AsParameters] Command command,
        ApplicationDbContext context,
        CurrentUserService currentUserService,
        CancellationToken ct)
    {
        var userId = currentUserService.GetCurrentUserId();
        var tag = await context.Tags
            .SingleOrDefaultAsync(t => t.Id == command.TagId && t.UserId == userId, ct);

        if (tag is null)
        {
            throw new TagNotFoundExceptions(command.TagId);
        }

        tag.Rename(command.Body.NewName);

        context.Tags.Update(tag);
        await context.SaveChangesAsync(ct);
    }

    [Validate]
    public sealed partial record Command : IValidationTarget<Command>
    {
        public required TagId TagId { get; init; }
        public required CommandBody Body { get; init; }

        [Validate]
        public sealed partial record CommandBody : IValidationTarget<CommandBody>
        {
            [NotEmpty] public required string NewName { get; init; }
        }
    }
}