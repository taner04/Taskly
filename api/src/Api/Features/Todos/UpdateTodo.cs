namespace Api.Features.Todos;

[Handler]
[MapPut("api/todos/update")]
public static partial class UpdateTodo
{
    internal static void CustomizeEndpoint(IEndpointConventionBuilder endpoint)
    {
        endpoint.WithTags(nameof(Todo));
    }

    private static async ValueTask<ErrorOr<Success>> HandleAsync(Command command, ApplicationDbContext context,
        CancellationToken ct)
    {
        var todo = await context.Todos.SingleOrDefaultAsync(
            t => t.Id == command.TodoId && t.UserId == command.UserId, ct);

        if (todo is null)
        {
            return Error.NotFound("Todo.NotFound",
                $"The todo does not exist with the specified id '{command.TodoId}'.");
        }

        var updateTodoResult =
            todo.Update(command.Title, command.Description, command.Priority, command.IsCompleted);

        if (updateTodoResult.IsError)
        {
            return updateTodoResult;
        }

        context.Todos.Update(todo);
        await context.SaveChangesAsync(ct);

        return Result.Success;
    }

    [Validate]
    public sealed partial record Command : UserRequest, IValidationTarget<Command>
    {
        public required Guid TodoId { get; init; }
        [NotEmpty] public required string Title { get; init; }
        [NotEmpty] public required string Description { get; init; }
        public required TodoPriority Priority { get; init; }
        public required bool IsCompleted { get; init; }
    }
}