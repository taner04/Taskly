using System.Diagnostics;
using ReminderService.Processors;

namespace ReminderService;

public sealed partial class Worker(
    SendReminderProcessor processor,
    IHostApplicationLifetime applicationLifetime,
    ILogger<Worker> logger)
    : BackgroundService
{
    public const string ActivitySourceName = "ReminderService";
    private readonly ActivitySource _activitySource = new(ActivitySourceName);

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
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
                    await processor.SendReminders(stoppingToken);
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