using System.Diagnostics;
using System.Linq.Expressions;
using Api.Features.Todos.Model;
using Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using MimeKit;

namespace ReminderService;

public sealed partial class Worker(
    IServiceProvider serviceProvider,
    IHostApplicationLifetime applicationLifetime,
    EmailService emailService,
    ILogger<Worker> logger)
    : BackgroundService
{
    public const string ActivitySourceName = "ReminderService";
    private readonly ActivitySource _activitySource = new(ActivitySourceName);

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        LogBeginHandling("WorkerStartup", nameof(Worker), "Startup");

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var activity = _activitySource.StartActivity("reminder-cycle", ActivityKind.Client);

                var sw = Stopwatch.StartNew();
                LogBeginHandling("ReminderCycle", nameof(Worker), activity?.Id ?? "none");

                try
                {
                    await SendReminders(context, stoppingToken);
                }
                catch (Exception ex)
                {
                    LogOccuredError("ReminderCycle", ex);
                    activity?.AddException(ex);
                }

                sw.Stop();
                LogFinishedHandling("ReminderCycle", sw.ElapsedMilliseconds);

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
        finally
        {
            LogFinishedHandling("WorkerStopping", 0);
            applicationLifetime.StopApplication();
        }
    }

    private static Expression<Func<Todo, bool>> ReminderDueExpression()
    {
        var now = DateTime.UtcNow;

        return t =>
            t.Deadline.HasValue &&
            t.ReminderOffsetInMinutes.HasValue &&
            t.Deadline.Value.AddMinutes(-t.ReminderOffsetInMinutes.Value) <= now &&
            t.IsCompleted == false;
    }

    private async Task SendReminders(
        ApplicationDbContext context,
        CancellationToken cancellationToken)
    {
        var sw = Stopwatch.StartNew();
        LogBeginHandling("SendReminders", nameof(Worker), DateTime.UtcNow.ToString("u"));

        var todos = await context.Todos
            .Where(ReminderDueExpression())
            .Select(t => new { Todo = t, t.UserId })
            .ToListAsync(cancellationToken);

        if (todos.Count == 0)
        {
            LogFinishedHandling("SendReminders", 0);
            return;
        }

        var groupedByUser = todos.GroupBy(t => t.UserId);

        foreach (var group in groupedByUser)
        {
            var userId = group.Key;
            var todosForUser = group.Select(x => x.Todo).ToList();

            LogBeginHandling("SendUserReminderBatch", nameof(Worker), $"UserId={userId}");

            var userEmail = await context.Users
                .Where(u => u.Id == userId)
                .Select(u => u.Email)
                .FirstOrDefaultAsync(cancellationToken);

            if (userEmail is null)
            {
                LogOccuredError("SendUserReminderBatch", new Exception($"User {userId} has no email."));
                continue;
            }

            try
            {
                var message = CreateMimeMessage(userEmail, todosForUser);
                await emailService.SendEmailAsync(message, cancellationToken);

                todosForUser.ForEach(t => t.ClearReminder());
                LogFinishedHandling("SendUserReminderBatch", 0);
            }
            catch (Exception ex)
            {
                LogOccuredError("SendUserReminderBatch", ex);
            }
        }

        await context.SaveChangesAsync(cancellationToken);

        sw.Stop();
        LogFinishedHandling("SendReminders", sw.ElapsedMilliseconds);
    }

    private static MimeMessage CreateMimeMessage(
        string userEmail,
        List<Todo> todos)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Reminder Service", "no-reply@taskly.com"));
        message.To.Add(MailboxAddress.Parse(userEmail));
        message.Subject = "Taskly - Upcoming todo reminders";

        message.Body = new BodyBuilder
        {
            HtmlBody = "<h1>Your upcoming todo reminders</h1><ul>" +
                       string.Join("", todos.Select(t =>
                           $"<li><strong>{t.Title}</strong>: Due at {t.Deadline:u}</li>")) +
                       "</ul>"
        }.ToMessageBody();

        return message;
    }

    [LoggerMessage(
        EventId = 0,
        Level = LogLevel.Information,
        Message = "Beginning {requestName} ({behaviorType}) with context {requestContext}")]
    private partial void LogBeginHandling(
        string requestName,
        string behaviorType,
        string requestContext);

    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Error,
        Message = "Error handling {requestName}")]
    private partial void LogOccuredError(
        string requestName,
        Exception exception);

    [LoggerMessage(
        EventId = 2,
        Level = LogLevel.Information,
        Message = "Finished handling {requestName} in {elapsedMs} ms.")]
    private partial void LogFinishedHandling(
        string requestName,
        long elapsedMs);
}