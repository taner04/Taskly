using Hangfire;
using Taskly.WebApi.Common.Infrastructure.Services.Emails;
using Taskly.WebApi.Features.Todos.Common.EmailTemplates;
using Taskly.WebApi.Features.Todos.Common.Exceptions;
using Taskly.WebApi.Features.Todos.Common.Extensions;
using Taskly.WebApi.Features.Todos.Common.Specifications;
using Taskly.WebApi.Features.Users.Common.Models;

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

    internal static Ok<GetTodoResponse> TransformResult(
        GetTodoResponse response) =>
        TypedResults.Ok(response);

    private static async ValueTask<GetTodoResponse> HandleAsync(
        [FromBody] Command command,
        TasklyDbContext context,
        CurrentUserService currentUserService,
        IBackgroundJobClient jobClient,
        EmailService emailService,
        CancellationToken ct)
    {
        InvalidTodoDeadlineException.ThrowIfInvalid(command.Body.Deadline, command.Body.ReminderOffsetInMinutes);

        var userId = currentUserService.GetUserId();

        var spec = new TodoByUserIdSpecification(command.TodoId, userId);
        var todo = await context.Todos
            .WithSpecification(spec)
            .SingleOrDefaultAsync(ct) ?? throw new ModelNotFoundException<Todo>(command.TodoId.Value);

        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId, ct) ??
                   throw new ModelNotFoundException<User>(userId.Value);

        todo.Deadline = command.Body.Deadline;
        todo.ReminderOffsetInMinutes = command.Body.ReminderOffsetInMinutes;

        var hangfireJobId = jobClient.Schedule(
            () => emailService.SendEmailAsync(new ReminderEmailTemplate(user.Email, todo), ct),
            TimeSpan.FromMinutes(todo.ReminderOffsetInMinutes!.Value));

        todo.HangfireJobId = hangfireJobId;

        context.Todos.Update(todo);
        await context.SaveChangesAsync(ct);

        return todo.ToGetTodoResponse();
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