using Api.Features.Tags.Model;
using Api.Features.Todos.Model;
using Api.Features.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.Todos;

[Handler]
[MapPost(ApiRoutes.Todos.AddTags)]
[Authorize]
public static partial class AddTagsToTodo
{
    internal static void CustomizeEndpoint(
        IEndpointConventionBuilder endpoint)
    {
        endpoint.WithTags(nameof(Todo));
    }

    internal static Results<Ok, NotFound<Error>, BadRequest<Error>> TransformResult(
        ErrorOr<Success> result)
    {
        return result.Match<Results<Ok, NotFound<Error>, BadRequest<Error>>>(
            _ => TypedResults.Ok(),
            error => TypedResults.NotFound(error.First()));
    }

    private static async ValueTask<ErrorOr<Success>> HandleAsync(
        [AsParameters] Command command,
        ApplicationDbContext context,
        CurrentUserService currentUserService,
        CancellationToken ct)
    {
        var userId = currentUserService.GetCurrentUserId();
        var todo = await context.Todos.SingleOrDefaultAsync(
            t => t.Id == command.TodoId && t.UserId == userId, ct);

        if (todo is null)
        {
            return Error.NotFound("Todo.NotFound",
                $"The todo does not exist with the specified id '{command.TodoId}'.");
        }

        var tags = await context.Tags
            .Where(tag => command.Body.TagIds.Contains(tag.Id) && tag.UserId == userId)
            .ToListAsync(ct);

        if (tags.Count == 0)
        {
            return Error.NotFound("Tag.NotFound", "No tags were found with the specified IDs.");
        }

        var existingTagIds = todo.Tags.Select(t => t.Id).ToList();
        foreach (var tag in tags.Where(tag => !existingTagIds.Contains(tag.Id)))
        {
            todo.Tags.Add(tag);
        }

        context.Todos.Update(todo);
        await context.SaveChangesAsync(ct);

        return Result.Success;
    }

    [Validate]
    public sealed partial record Command : IValidationTarget<Command>, ITransactionalRequest
    {
        [FromRoute] [NotEmpty] public required TodoId TodoId { get; init; }
        [NotNull] public required CommandBody Body { get; init; } = null!;

        [Validate]
        public sealed partial record CommandBody : IValidationTarget<CommandBody>
        {
            [NotEmpty] public required List<TagId> TagIds { get; init; }
        }
    }
}