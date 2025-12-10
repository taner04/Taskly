using System.Globalization;
using Api.Features.Todos.Exceptions;

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
        CurrentUserService current,
        CancellationToken ct)
    {
        var userId = current.GetUserId();

        var todo = await context.Todos
            .Include(t => t.Attachments)
            .SingleOrDefaultAsync(t => t.Id == command.TodoId && t.UserId == userId, ct);

        if (todo is null)
        {
            throw new ModelNotFoundException<Todo>(command.TodoId.Value);
        }
        
        if (!DateTime.TryParse(
                command.Body.Date,
                null,
                DateTimeStyles.RoundtripKind,
                out var date))
        {
            throw new TodoInvalidDeadline(command.Body.Date);
        }
        
        date = date.Kind switch
        {
            DateTimeKind.Utc => date,
            DateTimeKind.Local => date.ToUniversalTime(),
            DateTimeKind.Unspecified => DateTime.SpecifyKind(date, DateTimeKind.Utc),
            _ => date
        };

        todo.SetReminder(date, command.Body.ReminderOffsetInMinutes);
        
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
            public required string Date { get; init; }
            public required int ReminderOffsetInMinutes { get; init; }
        }
    }
}