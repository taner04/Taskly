using Api.Features.Todos.Specifications;
using Ardalis.Specification.EntityFrameworkCore;

namespace Api.Features.Todos.Endpoints;

[Handler]
[MapPut(Routes.Todos.UpdateReminder)]
[Authorize]
public static partial class UpdateReminder
{
    internal static void CustomizeEndpoint(
        IEndpointConventionBuilder endpoint)
    {
        endpoint.WithTags(nameof(Todo));
    }

    private static async ValueTask HandleAsync(
        Command command,
        ApplicationDbContext context,
        CurrentUserService currentUserService,
        CancellationToken ct)
    {
        var userId = currentUserService.GetUserId();
        var spec = new TodoByUserIdSpecification(command.TodoId, userId);
        var todo = await context.Todos
            .WithSpecification(spec)
            .SingleOrDefaultAsync(ct);
        
        if (todo is null)
        {
            throw new ModelNotFoundException<Todo>(command.TodoId.Value);
        }

        todo.SetReminder(command.Body.Date, command.Body.ReminderOffsetInMinutes);

        context.Todos.Update(todo);
        await context.SaveChangesAsync(ct);
    }

    [Validate]
    public sealed partial record Command : IValidationTarget<Command>
    {
        [NotEmpty] [FromRoute] public required TodoId TodoId { get; init; }
        [NotNull] public required CommandBody Body { get; init; } = null!;

        [Validate]
        public sealed partial record CommandBody : IValidationTarget<CommandBody>
        {
            public required DateTime Date { get; init; }
            public required int ReminderOffsetInMinutes { get; init; }

            private static void AdditionalValidations(
                ValidationResult errors,
                Command command
            )
            {
                if (command.Body.Date.Kind != DateTimeKind.Utc)
                {
                    errors.Add(
                        new ValidationError
                        {
                            PropertyName = "Date",
                            ErrorMessage = $"The date '{command.Body.Date}' is not a valid utc date."
                        }
                    );
                }
            }
        }
    }
}