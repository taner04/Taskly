using Hangfire;
using Taskly.WebApi.Common.Infrastructure.Services.Emails;
using Taskly.WebApi.Features.Todos.EmailTemplates;
using Taskly.WebApi.Features.Todos.Exceptions;

namespace Taskly.WebApi.Features.Todos.Endpoints;

[Handler]
[MapPut(ApiRoutes.Todos.UpdateReminder)]
[Authorize(Policy = Security.Policies.User)]
public static partial class UpdateReminder
{
    internal static void CustomizeEndpoint(
        RouteHandlerBuilder endpoint)
    {
        endpoint.WithTags(nameof(Todo));
        endpoint.RequireRateLimiting(Security.RateLimiting.Global);
    }

    private static async ValueTask HandleAsync(
        [FromBody] Command command,
        TasklyDbContext context,
        CurrentUserService currentUserService,
        IBackgroundJobClient jobClient,
        EmailService emailService,
        CancellationToken ct)
    {
        InvalidTodoDeadlineException.ThrowIfInvalid(command.Body.Deadline, command.Body.ReminderOffsetInMinutes);

        var userId = currentUserService.GetUserId();

        var user = await context.Users.Include(u => u.Todos)
            .FirstOrDefaultAsync(u => u.Id == userId, ct) ?? throw new ModelNotFoundException<User>(userId.Value);

        var todo = user.Todos.FirstOrDefault(t => t.Id == command.TodoId) ??
                   throw new ModelNotFoundException<Todo>(command.TodoId.Value);

        todo.Deadline = command.Body.Deadline;
        todo.ReminderOffsetInMinutes = command.Body.ReminderOffsetInMinutes;

        var hangfireJobId = jobClient.Schedule(
            () => emailService.SendEmailAsync(new ReminderEmailTemplate(user.Email, todo), ct),
            TimeSpan.FromMinutes(todo.ReminderOffsetInMinutes!.Value));

        todo.HangfireJobId = hangfireJobId;

        context.Todos.Update(todo);
        await context.SaveChangesAsync(ct);
    }

    [Validate]
    public sealed partial record Command : IValidationTarget<Command>
    {
        [NotEmpty] [FromRoute] public required TodoId TodoId { get; init; }
        [FromBody] [NotNull] public required CommandBody Body { get; init; } = null!;

        [Validate]
        public sealed partial record CommandBody : IValidationTarget<CommandBody>
        {
            public required DateTime Deadline { get; init; }
            public required int ReminderOffsetInMinutes { get; init; }
        }
    }
}