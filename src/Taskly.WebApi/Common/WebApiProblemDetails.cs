namespace Taskly.WebApi.Common;

public sealed class WebApiProblemDetails : ProblemDetails
{
    public string? ErrorCode { get; init; }
    public Dictionary<string, string[]> Errors { get; init; } = [];
}