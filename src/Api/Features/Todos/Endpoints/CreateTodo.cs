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

    internal static Created<Response> TransformResult(
        Response response)
    {
        return TypedResults.Created($"api/todos/{response.TodoId}", response);
    }

    private static async ValueTask<Response> HandleAsync(
        Command command,
        ApplicationDbContext context,
        CurrentUserService currentUserService,
        CancellationToken ct)
    {
        var userId = currentUserService.GetUserId();

        var newTodo = Todo.Create(command.Title, command.Description, command.Priority, userId);

        await context.Todos.AddAsync(newTodo, ct);
        await context.SaveChangesAsync(ct);

        return new Response(newTodo.Id.Value);
    }

    [Validate]
    public sealed partial record Command : IValidationTarget<Command>
    {
        [NotEmpty] public required string Title { get; init; }
        public string? Description { get; init; }
        public required TodoPriority Priority { get; init; }
    }

    public sealed record Response(Guid TodoId);
}