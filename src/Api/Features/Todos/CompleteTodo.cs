using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.Todos;

[Handler]
[MapPost(ApiRoutes.Todos.Complete)]
[Authorize]
public static partial class CompleteTodo
{
    internal static void CustomizeEndpoint(IEndpointConventionBuilder endpoint)
    {
        endpoint.WithTags(nameof(Todo));
    }

    private static async ValueTask<ErrorOr<Success>> HandleAsync(
        [AsParameters] Command command,
        ApplicationDbContext context,
        CancellationToken ct)
    {
        var todo = await context.Todos.SingleOrDefaultAsync(
            t => t.Id == command.TodoId && t.UserId == command.UserId, ct);

        if (todo is null)
        {
            return Error.NotFound("Todo.NotFound",
                $"The todo does not exist with the specified id '{command.TodoId}'.");
        }

        todo.SetCompletionStatus(command.Completed);

        context.Todos.Update(todo);
        await context.SaveChangesAsync(ct);

        return Result.Success;
    }

    [Validate]
    public sealed partial record Command : UserRequest, IValidationTarget<Command>
    {
        [FromRoute] public required Guid TodoId { get; init; }
        public bool Completed { get; init; }
    }
}