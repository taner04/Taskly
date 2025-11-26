using System.Diagnostics;

namespace Api.Behaviors;

public sealed partial class LoggingBehavior<TMessage, TResponse>(
    ILogger<LoggingBehavior<TMessage, TResponse>> logger,
    IHttpContextAccessor httpContextAccessor
) : Behavior<TMessage, TResponse>
{
    public override async ValueTask<TResponse> HandleAsync(TMessage request, CancellationToken cancellationToken)
    {
        var scopeProps = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["RequestData"] = request,
            ["HandlerType"] = HandlerType.Name
        };

        var httpContext = httpContextAccessor.HttpContext;

        if (httpContext is not null)
        {
            var user = httpContext.User?.Identity;

            scopeProps["UserName"] = user?.Name;
            scopeProps["IsAuthenticated"] = user?.IsAuthenticated;
            scopeProps["RemoteIP"] = httpContext.Connection.RemoteIpAddress?.ToString();

            var req = httpContext.Request;

            scopeProps["Method"] = req.Method;
            scopeProps["Path"] = req.Path.ToString();
            scopeProps["QueryString"] = req.QueryString.Value;
            scopeProps["UserAgent"] = req.Headers.UserAgent.ToString();
            scopeProps["ConnectingIP"] = req.Headers["CF-Connecting-IP"].ToString();
            scopeProps["TraceIdentifier"] = httpContext.TraceIdentifier;
        }

        using var scope = logger.BeginScope(scopeProps);
        var sw = Stopwatch.StartNew();

        try
        {
            var response = await Next(request, cancellationToken);
            sw.Stop();

            LogSuccess(logger, HandlerType, sw.Elapsed.TotalMilliseconds);

            return response;
        }
        catch (Exception ex)
        {
            sw.Stop();

            LogException(logger, HandlerType, sw.Elapsed.TotalMilliseconds, ex);

            throw;
        }
    }

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Executed {Type} handler in {Elapsed} ms"
    )]
    private static partial void LogSuccess(
        ILogger logger,
        Type type,
        double elapsed);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Exception during {Type} handler after {Elapsed} ms"
    )]
    private static partial void LogException(
        ILogger logger,
        Type type,
        double elapsed,
        Exception exception);
}