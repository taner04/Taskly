using Api.Features.Todos.Model;
using Api.Features.Users.Model;
using Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ReminderService.Services;

public sealed class TodoService(ApplicationDbContext context)
{
    internal async Task<IReadOnlyList<UserReminderBatch>> GetReminderBatchesAsync(
        CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        return await context.Todos
            .Include(t => t.User)
            .Where(t =>
                t.Deadline.HasValue &&
                t.ReminderOffsetInMinutes.HasValue &&
                !t.IsCompleted &&
                t.Deadline.Value.AddMinutes(-t.ReminderOffsetInMinutes.Value) <= now)
            .Select(t => new
            {
                t.UserId,
                t.User.Email,
                Todo = t
            })
            .GroupBy(x => new { x.UserId, x.Email })
            .Select(g => new UserReminderBatch(
                g.Key.UserId,
                g.Key.Email!,
                g.Select(x => x.Todo).ToList()))
            .ToListAsync(ct);
    }

    internal sealed record UserReminderBatch(
        UserId UserId,
        string Email,
        List<Todo> Todos);
}