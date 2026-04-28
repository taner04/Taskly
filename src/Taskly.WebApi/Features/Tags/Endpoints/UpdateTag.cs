using Taskly.Shared.WebApi.Responses.Tags;
using Taskly.WebApi.Features.Tags.Common.Exceptions;
using Taskly.WebApi.Features.Tags.Common.Specifications;
using TagId = Taskly.WebApi.Features.Tags.Common.Models.TagId;

namespace Taskly.WebApi.Features.Tags.Endpoints;

[Handler]
[MapPut(ApiRoutes.Tags.Update)]
[Authorize(Policy = Security.Policies.User)]
public static partial class UpdateTag
{
    internal static void CustomizeEndpoint(
        RouteHandlerBuilder endpoint)
    {
        endpoint.WithTags(nameof(Tag));
        endpoint.RequireRateLimiting(Security.RateLimiting.Global);
    }

    internal static Ok<GetTagResponse> TransformResult(
        GetTagResponse result) =>
        TypedResults.Ok(result);

    private static async ValueTask<GetTagResponse> HandleAsync(
        [AsParameters] Command command,
        TasklyDbContext context,
        CurrentUserService currentUserService,
        CancellationToken ct)
    {
        InvalidTagNameException.ThrowIfInvalid(command.Body.NewName);

        var userId = currentUserService.GetUserId();

        var spec = new TagByIdSpecification(command.TagId, userId);
        var tag = await context.Tags
            .WithSpecification(spec)
            .SingleOrDefaultAsync(ct) ?? throw new ModelNotFoundException<Tag>(command.TagId.Value);


        tag.Name = command.Body.NewName;

        context.Tags.Update(tag);
        await context.SaveChangesAsync(ct);

        return new GetTagResponse(tag.Id.Value, tag.Name);
    }

    [Validate]
    public sealed partial record Command : IValidationTarget<Command>
    {
        [FromRoute] [NotEmpty] public required TagId TagId { get; init; }
        [FromBody] [NotNull] public required CommandBody Body { get; init; }

        [Validate]
        public sealed partial record CommandBody : IValidationTarget<CommandBody>
        {
            [NotEmpty] public required string NewName { get; init; }
        }
    }
}