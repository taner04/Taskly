using Api.Features.Shared.Api;
using Api.Features.Todos.Model;
using Api.Features.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.Todos;

[Handler]
[MapPut(Routes.Todos.Update)]
[Authorize]
public static partial class UpdateTodo
{
    internal static void CustomizeEndpoint(
        IEndpointConventionBuilder endpoint)
    {
        endpoint.WithTags(nameof(Todo));
    }

    internal static Results<Ok, NotFound<Error>, BadRequest<Error>> TransformResult(
        ErrorOr<Success> result)
    {
        if (result.FirstError.Type == ErrorType.NotFound)
        {
            return result.Match<Results<Ok, NotFound<Error>, BadRequest<Error>>>(
                _ => TypedResults.Ok(),
                error => TypedResults.NotFound(error.First()));
        }

        return result.Match<Results<Ok, NotFound<Error>, BadRequest<Error>>>(
            _ => TypedResults.Ok(),
            error => TypedResults.BadRequest(error.First()));
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

        var updateTodoResult =
            todo.Update(command.Body.Title, command.Body.Description, command.Body.Priority);

        if (updateTodoResult.IsError)
        {
            return updateTodoResult;
        }

        context.Todos.Update(todo);
        await context.SaveChangesAsync(ct);

        return Result.Success;
    }

    [Validate]
    public sealed partial record Command : IValidationTarget<Command>, ITransactionalRequest
    {
        [FromRoute] [NotEmpty] public required TodoId TodoId { get; init; }
        [NotNull] public CommandBody Body { get; init; } = null!;

        [Validate]
        public sealed partial record CommandBody : IValidationTarget<CommandBody>
        {
            [NotEmpty] public required string Title { get; init; }
            public string? Description { get; init; }
            public required TodoPriority Priority { get; init; }
        }
    }
}