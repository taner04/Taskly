using Api.Features.Tags.Specifications;
using Ardalis.Specification.EntityFrameworkCore;

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
        var spec = new TagByIdSpecification(command.TagId, currentUserService.GetUserId());
        var tag = await context.Tags
            .WithSpecification(spec)
            .SingleOrDefaultAsync(ct);

        if (tag is null)
        {
            throw new ModelNotFoundException<Tag>(command.TagId.Value);
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