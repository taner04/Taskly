using Api.Features.Todos.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Features.Todos;

[Handler]
[MapPost(ApiRoutes.Todos.Create)]
[Authorize]
public static partial class CreateTodo
{
    internal static void CustomizeEndpoint(
        IEndpointConventionBuilder endpoint)
    {
        endpoint.WithTags(nameof(Todo));
    }

    internal static Results<Ok, BadRequest<Error>> TransformResult(
        ErrorOr<Success> result)
    {
        return result.Match<Results<Ok, BadRequest<Error>>>(
            _ => TypedResults.Ok(),
            error => TypedResults.BadRequest(error.First()));
    }

    private static async ValueTask<ErrorOr<Success>> HandleAsync(
        Command command,
        ApplicationDbContext context,
        CurrentUserService currentUserService,
        CancellationToken ct)
    {
        var userId = currentUserService.GetCurrentUserId();
        var createNewTodoResult = Todo.TryCreate(command.Title, command.Description, command.Priority, userId);

        if (createNewTodoResult.IsError)
        {
            return createNewTodoResult.Errors;
        }

        var newTodo = createNewTodoResult.Value;

        context.Todos.Add(newTodo!);
        await context.SaveChangesAsync(ct);

        return Result.Success;
    }

    [Validate]
    public sealed partial record Command : IValidationTarget<Command>, ITransactionalRequest
    {
        [NotEmpty] public required string Title { get; init; }
        public string? Description { get; init; }
        public required TodoPriority Priority { get; init; }
    }
}