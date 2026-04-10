namespace Taskly.WebApi.Common.Composition.Options;

/// <summary>
///     Configuration options for the reminder background service.
/// </summary>
public sealed class ReminderOptions
{
    /// <summary>
    ///     Gets or sets the interval in minutes between reminder cycles.
    ///     Default is 5 minutes.
    /// </summary>
    public int CheckIntervalMinutes { get; set; } = 5;

    /// <summary>
    ///     Gets or sets the maximum number of reminder batches to process per cycle.
    ///     Default is 100.
    /// </summary>
    public int MaxBatchSize { get; set; } = 100;

    /// <summary>
    ///     Gets or sets the maximum number of todo items per email reminder.
    ///     Default is 50.
    /// </summary>
    public int MaxTodosPerEmail { get; set; } = 50;
}