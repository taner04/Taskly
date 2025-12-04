using Api.Features.Todos.Exceptions;
using Api.Features.Todos.Model;
using Api.Features.Users.Services;
using Api.Shared.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.Todos.Endpoints;

[Handler]
[MapDelete(Routes.Todos.Remove)]
[Authorize]
public static partial class RemoveTodo
{
    internal static void CustomizeEndpoint(
        IEndpointConventionBuilder endpoint)
    {
        endpoint.WithTags(nameof(Todo));
    }

    internal static NoContent TransformResult(
        ValidationResult result)
    {
        return TypedResults.NoContent();
    }

    private static async ValueTask HandleAsync(
        Command command,
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

        context.Todos.Remove(todo);
        await context.SaveChangesAsync(ct);
    }

    [Validate]
    public sealed partial record Command : IValidationTarget<Command>
    {
        [FromRoute] [NotEmpty] public required TodoId TodoId { get; init; }
    }
}