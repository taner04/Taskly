using Api.Features.Tags.Exceptions;
using Api.Features.Tags.Model;
using Api.Features.Todos.Exceptions;
using Api.Features.Todos.Model;
using Api.Features.Users.Services;
using Api.Shared.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.Todos.Endpoints;

[Handler]
[MapDelete(Routes.Todos.RemoveTag)]
[Authorize]
public static partial class RemoveTag
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
            throw new TodoNotFoundException(command.TodoId);
        }

        var tagToRemove = todo.Tags.SingleOrDefault(t => t.Id == command.TagId);

        if (tagToRemove is null)
        {
            throw new TagNotFoundExceptions(command.TagId);
        }

        todo.Tags.Remove(tagToRemove);
    }

    [Validate]
    public sealed partial record Command : IValidationTarget<Command>
    {
        [FromRoute] [NotEmpty] public required TodoId TodoId { get; init; }
        [FromRoute] [NotEmpty] public required TagId TagId { get; init; }
    }
}