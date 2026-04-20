namespace Taskly.WebApi.Client.Common;

public sealed class WebClientOptions
{
    public Uri BaseAddress { get; set; } = null!;
    public TimeSpan Timeout { get; set; }
}