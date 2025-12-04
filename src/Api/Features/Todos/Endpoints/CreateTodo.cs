using Api.Features.Todos.Model;
using Api.Features.Users.Services;
using Api.Shared.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Features.Todos.Endpoints;

[Handler]
[MapPost(Routes.Todos.Create)]
[Authorize]
public static partial class CreateTodo
{
    internal static void CustomizeEndpoint(
        IEndpointConventionBuilder endpoint)
    {
        endpoint.WithTags(nameof(Todo));
    }

    internal static Created<Dto> TransformResult(
        Dto dto)
    {
        return TypedResults.Created($"api/todos/{dto.TodoId}", dto);
    }

    private static async ValueTask<Dto> HandleAsync(
        Command command,
        ApplicationDbContext context,
        CurrentUserService currentUserService,
        CancellationToken ct)
    {
        var userId = currentUserService.GetCurrentUserId();

        var newTodo = new Todo(command.Title, command.Description, command.Priority, userId);

        await context.Todos.AddAsync(newTodo, ct);
        await context.SaveChangesAsync(ct);

        return new Dto(newTodo.Id.Value);
    }

    [Validate]
    public sealed partial record Command : IValidationTarget<Command>
    {
        [NotEmpty] public required string Title { get; init; }
        public string? Description { get; init; }
        public required TodoPriority Priority { get; init; }
    }

    public sealed record Dto(Guid TodoId);
}