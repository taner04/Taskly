using Api.Features.Tags.Model;
using Api.Features.Todos.Model;
using Api.Features.Users;
using Api.Shared.Features.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.Todos;

[Handler]
[MapDelete(Routes.Todos.RemoveTag)]
[Authorize]
public static partial class RemoveTagFromTodo
{
    internal static void CustomizeEndpoint(
        IEndpointConventionBuilder endpoint)
    {
        endpoint.WithTags(nameof(Todo));
    }

    internal static Results<Ok, NotFound<Error>> TransformResult(
        ErrorOr<Success> result)
    {
        return result.Match<Results<Ok, NotFound<Error>>>(
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
        var todo = await context.Todos
            .Include(t => t.Tags)
            .SingleOrDefaultAsync(t => t.Id == command.TodoId && t.UserId == userId, ct);

        if (todo is null)
        {
            return Error.NotFound("Todo.NotFound",
                $"The todo does not exist with the specified id '{command.TodoId}'.");
        }

        var tagToRemove = todo.Tags.SingleOrDefault(t => t.Id == command.TagId);

        if (tagToRemove is null)
        {
            return Error.NotFound("Todo.TagNotFound",
                $"The tag '{command.TagId}' is not associated with this todo.");
        }

        todo.Tags.Remove(tagToRemove);

        return Result.Success;
    }

    [Validate]
    public sealed partial record Command : IValidationTarget<Command>, ITransactionalRequest
    {
        [FromRoute] [NotEmpty] public required TodoId TodoId { get; init; }
        [FromRoute] [NotEmpty] public required TagId TagId { get; init; }
    }
}