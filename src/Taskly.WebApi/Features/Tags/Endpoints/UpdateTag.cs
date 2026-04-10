using Ardalis.Specification.EntityFrameworkCore;
using Taskly.WebApi.Common.Infrastructure.Persistence;
using Taskly.WebApi.Common.Shared;
using Taskly.WebApi.Common.Shared.Exceptions;
using Taskly.WebApi.Features.Tags.Models;
using Taskly.WebApi.Features.Tags.Specifications;
using TagId = Taskly.WebApi.Features.Tags.Models.TagId;

namespace Taskly.WebApi.Features.Tags.Endpoints;

[Handler]
[MapPut(ApiRoutes.Tags.Update)]
[Authorize(Policy = Policies.Roles.User)]
public static partial class UpdateTag
{
    internal static void CustomizeEndpoint(
        IEndpointConventionBuilder endpoint)
    {
        endpoint.WithTags(nameof(Tag));
        endpoint.RequireRateLimiting(Policies.RateLimiting.Global);
    }

    private static async ValueTask HandleAsync(
        [AsParameters] Command command,
        TasklyDbContext context,
        CurrentUserService currentUserService,
        CancellationToken ct)
    {
        var userId = currentUserService.GetUserId();

        var spec = new TagByIdSpecification(command.TagId, userId);
        var tag = await context.Tags
            .WithSpecification(spec)
            .SingleOrDefaultAsync(ct) ?? throw new ModelNotFoundException<Tag>(command.TagId.Value);

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