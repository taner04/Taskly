using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.Todos;

[Handler]
[MapDelete(ApiRoutes.Todos.Delete)]
[Authorize]
public static partial class DeleteTodo
{
    internal static void CustomizeEndpoint(IEndpointConventionBuilder endpoint)
    {
        endpoint.WithTags(nameof(Todo));
    }
    
    internal static Results<Ok, NotFound<Error>> TransformResult(ErrorOr<Success> result)
    {
        return result.Match<Results<Ok, NotFound<Error>>>(
            _ => TypedResults.Ok(),
            error => TypedResults.NotFound(error.First()));
    }

    private static async ValueTask<ErrorOr<Success>> HandleAsync(
        Command command,
        ApplicationDbContext context,
        CurrentUserService currentUserService,
        CancellationToken ct)
    {
        var todo = await context.Todos.SingleOrDefaultAsync(
            t => t.Id == command.TodoId, ct);

        if (todo is null)
        {
            return Error.NotFound("Todo.NotFound",
                $"The todo does not exist with the specified id '{command.TodoId}'.");
        }

        var userId = currentUserService.GetCurrentUserId();
        if (todo.UserId != userId)
        {
            return Error.NotFound("Todo.IncorrectUser",
                $"The todo with id '{command.TodoId}' does not belong to the current user.");
        }

        context.Todos.Remove(todo);
        await context.SaveChangesAsync(ct);

        return Result.Success;
    }

    [Validate]
    public partial record Command : IValidationTarget<Command>
    {
        [FromRoute] [NotEmpty] public required TodoId TodoId { get; init; }
    }
}