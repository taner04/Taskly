namespace Taskly.WebApi.Common;

public sealed class ApiProblemDetails : ProblemDetails
{
    public string? ErrorCode { get; init; }
    public Dictionary<string, string[]> Errors { get; init; } = [];
}