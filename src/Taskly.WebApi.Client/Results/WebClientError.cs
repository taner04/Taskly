using Taskly.WebApi.Common;

namespace Taskly.WebApi.Client.Results;

public readonly struct WebClientError
{
    public string ErrorCode { get; }
    public string Title { get; }
    public string Detail { get; }
    public Dictionary<string, string[]> Errors { get; }

    private WebClientError(
        string errorCode,
        string title,
        string detail,
        Dictionary<string, string[]>? errors)
    {
        ErrorCode = errorCode;
        Title = title;
        Detail = detail;
        Errors = errors ?? [];
    }

    internal static WebClientError FromProblemdetails(WebApiProblemDetails problemDetails) =>
        new(
            problemDetails.ErrorCode!, problemDetails.Title!, problemDetails.Detail!, problemDetails.Errors);

    internal static WebClientError CustomError(string errorCode, string title, string detail) =>
        new(errorCode, title, detail, null);
}