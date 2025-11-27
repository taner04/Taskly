using System.Diagnostics;

namespace Api.Behaviors.Logger;

public sealed partial class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger,
    IHttpContextAccessor httpContextAccessor
) : Behavior<TRequest, TResponse>
{
    private readonly string _requestName = typeof(TRequest).Name;

    public override async ValueTask<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            var ctx = LoggerRequestContext.FromHttpContext(httpContextAccessor.HttpContext);
            LogBeginHandling(_requestName, GetType().Name, ctx);

            return await Next(request, cancellationToken);
        }
        catch (Exception e)
        {
            LogOccuredError(_requestName, e);
            throw;
        }
        finally
        {
            sw.Stop();
            LogFinishedHandling(_requestName, sw.ElapsedMilliseconds);
        }
    }

    [LoggerMessage(0, LogLevel.Information, "Beginning {requestName} in {behaviorType} with context {@requestContext}")]
    private partial void LogBeginHandling(string requestName, string behaviorType,
        [LogProperties] LoggerRequestContext requestContext);


    [LoggerMessage(1, LogLevel.Error, "Error handling {requestName}")]
    private partial void LogOccuredError(string requestName, Exception exception);

    [LoggerMessage(2, LogLevel.Information, "Finished handling {requestName} in {elapsedMs} ms.")]
    private partial void LogFinishedHandling(string requestName, long elapsedMs);
}