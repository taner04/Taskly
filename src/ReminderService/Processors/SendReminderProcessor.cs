using System.Diagnostics;
using Api.Infrastructure.Data;
using ReminderService.Data;
using ReminderService.Emails;

namespace ReminderService.Processors;

public sealed partial class SendReminderProcessor(
    ILogger<SendReminderProcessor> logger,
    ApplicationDbContext context,
    TodoService todoService,
    EmailService emailService)
{
    public async Task SendReminders(
        CancellationToken ct)
    {
        var sw = Stopwatch.StartNew();
        LogBeginHandling("SendReminders", nameof(SendReminderProcessor), DateTime.UtcNow.ToString("u"));

        var batches = await todoService.GetReminderBatchesAsync(ct);

        foreach (var batch in batches)
        {
            LogBeginHandling(
                "SendUserReminderBatch",
                nameof(SendReminderProcessor),
                $"UserId={batch.UserId}");

            try
            {
                var message = EmailBuilder.Build(batch.Email, batch.Todos);
                await emailService.SendEmailAsync(message, ct);

                batch.Todos.ForEach(t => t.ClearReminder());

                LogFinishedHandling("SendUserReminderBatch", 0);
            }
            catch (Exception ex)
            {
                LogOccuredError("SendUserReminderBatch", ex);
            }
        }

        await context.SaveChangesAsync(ct);

        sw.Stop();
        LogFinishedHandling("SendReminders", sw.ElapsedMilliseconds);
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