namespace Api.Behaviors.Logger;

public sealed class LoggerRequestContext
{
    private const string Unknown = "Unknown";

    private LoggerRequestContext(
        string path,
        string method,
        string? userId,
        string? userName,
        string? traceId,
        string? ipAddress,
        string? userAgent,
        string? queryString)
    {
        Path = path;
        Method = method;
        UserId = userId ?? Unknown;
        UserName = userName ?? Unknown;
        TraceId = traceId ?? Unknown;
        IpAddress = ipAddress ?? Unknown;
        UserAgent = userAgent ?? Unknown;
        QueryString = queryString ?? Unknown;
    }

    public string Path { get; }
    public string Method { get; }
    public string UserId { get; }
    public string UserName { get; }
    public string TraceId { get; }
    public string IpAddress { get; }
    public string? UserAgent { get; }
    public string? QueryString { get; }

    public static LoggerRequestContext FromHttpContext(
        HttpContext? httpContext)
    {
        if (httpContext is null)
        {
            return new LoggerRequestContext(
                Unknown,
                Unknown,
                Unknown,
                Unknown,
                Unknown,
                Unknown,
                Unknown,
                Unknown
            );
        }

        return new LoggerRequestContext(
            httpContext.Request.Path,
            httpContext.Request.Method,
            httpContext.User.FindFirst("sub")?.Value,
            httpContext.User.Identity?.Name,
            httpContext.TraceIdentifier,
            httpContext.Connection.RemoteIpAddress?.ToString(),
            httpContext.Request.Headers.UserAgent.ToString(),
            httpContext.Request.QueryString.ToString()
        );
    }
}