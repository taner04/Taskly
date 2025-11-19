namespace Api.Features.Todos;

[Handler]
[MapPost("api/todoss/create")]
public static partial class CreateTodo
{
    internal static void CustomizeEndpoint(IEndpointConventionBuilder endpoint)
    {
        endpoint.WithTags(nameof(Todo));
    }

    private static async ValueTask<ErrorOr<Success>> HandleAsync(Command command, ApplicationDbContext context,
        CancellationToken ct)
    {
        var createNewTodoResult = Todo.TryCreate(command.Title, command.Description, command.Priority, command.UserId);

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
    public sealed partial record Command : UserRequest, IValidationTarget<Command>
    {
        [NotEmpty] public required string Title { get; init; }
        [NotEmpty] public required string Description { get; init; }
        public required TodoPriority Priority { get; init; }
    }
}