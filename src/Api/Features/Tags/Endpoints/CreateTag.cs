using Api.Features.Tags.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Features.Tags.Endpoints;

[Handler]
[MapPost(Routes.Tags.Create)]
[Authorize]
public static partial class CreateTag
{
    internal static void CustomizeEndpoint(
        IEndpointConventionBuilder endpoint)
    {
        endpoint.WithTags(nameof(Tag));
    }

    internal static Created<Dto> TransformResult(
        Dto dto)
    {
        return TypedResults.Created($"api/todos/{dto.TagId}", dto);
    }

    private static async ValueTask<Dto> HandleAsync(
        Command command,
        ApplicationDbContext context,
        CurrentUserService currentUserService,
        CancellationToken ct)
    {
        var userId = currentUserService.GetCurrentUserId();
        var newTag = new Tag(command.TagName, userId);

        if (await context.Tags.AnyAsync(t => t.Name == newTag.Name && t.UserId == userId, ct))
        {
            throw new TagAlreadyExistsException(newTag.Name);
        }

        context.Tags.Add(newTag);
        await context.SaveChangesAsync(ct);

        return new Dto(newTag.Id.Value);
    }

    [Validate]
    public sealed partial record Command : IValidationTarget<Command>
    {
        [NotEmpty] public required string TagName { get; init; }
    }

    public sealed record Dto(Guid TagId);
}