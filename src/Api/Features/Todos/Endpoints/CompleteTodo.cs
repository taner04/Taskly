using Api.Features.Todos.Exceptions;
using Api.Features.Todos.Model;
using Api.Features.Users.Services;
using Api.Shared.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.Todos.Endpoints;

[Handler]
[MapPost(Routes.Todos.Complete)]
[Authorize]
public static partial class CompleteTodo
{
    internal static void CustomizeEndpoint(
        IEndpointConventionBuilder endpoint)
    {
        endpoint.WithTags(nameof(Todo));
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
            throw new TodoNotFoundException(command.TodoId);
        }

        todo.SetCompletionStatus(command.Body.Completed);

        context.Todos.Update(todo);
        await context.SaveChangesAsync(ct);

        return Result.Success;
    }

    [Validate]
    public sealed partial record Command : IValidationTarget<Command>
    {
        [FromRoute] [NotEmpty] public required TodoId TodoId { get; init; }
        [NotNull] public required CommandBody Body { get; init; } = null!;

        [Validate]
        public sealed partial record CommandBody : IValidationTarget<CommandBody>
        {
            public required bool Completed { get; init; }
        }
    }
}