using Hangfire;
using Taskly.WebApi.Features.Todos.Common.Extensions;
using Taskly.WebApi.Features.Todos.Common.Specifications;

namespace Taskly.WebApi.Features.Todos.Endpoints;

[Handler]
[MapDelete(ApiRoutes.Todos.RemoveReminder)]
[Authorize(Policy = Security.Policies.User)]
public static partial class RemovReminder
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
        [AsParameters] Command command,
        TasklyDbContext context,
        CurrentUserService currentUserService,
        IBackgroundJobClient jobClient,
        ILoggerFactory loggerFactory,
        CancellationToken ct)
    {
        var userId = currentUserService.GetUserId();

        var spec = new TodoByUserIdSpecification(command.TodoId, userId);
        var todo = await context.Todos
            .WithSpecification(spec)
            .SingleOrDefaultAsync(ct) ?? throw new ModelNotFoundException<Todo>(command.TodoId.Value);

        var jobId = todo.HangfireJobId;
        if (!string.IsNullOrEmpty(jobId))
        {
            todo.Deadline = null;
            todo.ReminderOffsetInMinutes = null;
            todo.HangfireJobId = null;

            _ = jobClient.Delete(jobId);
        }
        else
        {
            loggerFactory.CreateLogger(nameof(RemovReminder))
                .LogWarning("Todo with id {TodoId} did not have a reminder set, so no Hangfire job was deleted.",
                    command.TodoId.Value);
        }

        context.Update(todo);
        await context.SaveChangesAsync(ct);

        return todo.ToGetTodoResponse();
    }

    [Validate]
    public sealed partial record Command : IValidationTarget<Command>
    {
        [NotEmpty] [FromRoute] public required TodoId TodoId { get; init; }
    }
}