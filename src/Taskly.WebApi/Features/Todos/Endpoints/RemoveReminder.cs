using Ardalis.Specification.EntityFrameworkCore;
using Hangfire;
using Taskly.WebApi.Common.Infrastructure.Persistence;
using Taskly.WebApi.Common.Shared;
using Taskly.WebApi.Common.Shared.Exceptions;
using Taskly.WebApi.Features.Todos.Models;
using Taskly.WebApi.Features.Todos.Specifications;
using TodoId = Taskly.WebApi.Features.Todos.Models.TodoId;

namespace Taskly.WebApi.Features.Todos.Endpoints;

[Handler]
[MapDelete(ApiRoutes.Todos.RemoveReminder)]
[Authorize(Policy = Policies.Roles.User)]
public static partial class RemovReminder
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
        CancellationToken ct)
    {
        var userId = currentUserService.GetUserId();

        var spec = new TodoByUserIdSpecification(command.TodoId, userId);
        var todo = await context.Todos
            .WithSpecification(spec)
            .SingleOrDefaultAsync(ct) ?? throw new ModelNotFoundException<Todo>(command.TodoId.Value);

        var hangfireJobId = todo.HangfireJobId;

        todo.ClearReminder();
        _ = jobClient.Delete(hangfireJobId);

        context.Update(todo);
        await context.SaveChangesAsync(ct);
    }

    [Validate]
    public sealed partial record Command : IValidationTarget<Command>
    {
        [NotEmpty] [FromRoute] public required TodoId TodoId { get; init; }
    }
}