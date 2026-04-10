using Hangfire;
using Taskly.WebApi.Common.Infrastructure.Persistence;
using Taskly.WebApi.Common.Infrastructure.Services.Emails;
using Taskly.WebApi.Common.Shared;
using Taskly.WebApi.Common.Shared.Exceptions;
using Taskly.WebApi.Features.Todos.EmailTemplates;
using Taskly.WebApi.Features.Todos.Models;
using Taskly.WebApi.Features.Users.Models;
using TodoId = Taskly.WebApi.Features.Todos.Models.TodoId;

namespace Taskly.WebApi.Features.Todos.Endpoints;

[Handler]
[MapPut(ApiRoutes.Todos.UpdateReminder)]
[Authorize(Policy = Policies.Roles.User)]
public static partial class UpdateReminder
{
    internal static void CustomizeEndpoint(
        IEndpointConventionBuilder endpoint)
    {
        endpoint.WithTags(nameof(Todo));
        endpoint.RequireRateLimiting(Policies.RateLimiting.Global);
    }

    private static async ValueTask HandleAsync(
        Command command,
        TasklyDbContext context,
        CurrentUserService currentUserService,
        IBackgroundJobClient jobClient,
        EmailService emailService,
        CancellationToken ct)
    {
        var userId = currentUserService.GetUserId();

        var user = await context.Users.Include(u => u.Todos)
            .FirstOrDefaultAsync(u => u.Id == userId, ct) ?? throw new ModelNotFoundException<User>(userId.Value);

        var todo = user.Todos.FirstOrDefault(t => t.Id == command.TodoId) ??
                   throw new ModelNotFoundException<Todo>(command.TodoId.Value);

        var hangFireJobId =
            jobClient.Schedule(() => emailService.SendEmailAsync(new ReminderEmailTemplate(user.Email, todo), ct),
                TimeSpan.FromMinutes(todo.ReminderOffsetInMinutes!.Value));
        todo.SetReminder(command.Body.Date, command.Body.ReminderOffsetInMinutes, hangFireJobId);

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
        }
    }
}