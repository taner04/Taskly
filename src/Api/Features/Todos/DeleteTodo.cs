using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.Todos;

[Handler]
[MapDelete("api/todos/{todoId:guid}")]
[Authorize]
public static partial class DeleteTodo
{
    internal static void CustomizeEndpoint(IEndpointConventionBuilder endpoint)
    {
        endpoint.WithTags(nameof(Todo));
    }

    private static async ValueTask<ErrorOr<Success>> HandleAsync(
        Command command,
        ApplicationDbContext context,
        CancellationToken ct)
    {
        var taskItem = await context.Todos.SingleOrDefaultAsync(
            t => t.Id == command.TodoId && t.UserId == command.UserId, ct);

        if (taskItem is null)
        {
            return Error.NotFound("TaskItem.NotFound", $"The task with the Id '{command.TodoId}' was not found.");
        }

        context.Todos.Remove(taskItem);
        await context.SaveChangesAsync(ct);

        return Result.Success;
    }

    [Validate]
    public partial record Command : UserRequest, IValidationTarget<Command>
    {
        [FromRoute] [NotEmpty] public required TodoId TodoId { get; init; }
    }
}