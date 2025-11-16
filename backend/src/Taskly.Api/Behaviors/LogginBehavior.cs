using System.Diagnostics;
using Mediator;
using Taskly.Api.Abstractions;

namespace Taskly.Api.Behaviors;

public sealed class LoggingBehaviour<TMessage, TResponse>(
    ILogger<LoggingBehaviour<TMessage, TResponse>> logger)
    : IPipelineBehavior<TMessage, TResponse>
    where TMessage : IMessage
{
    public async ValueTask<TResponse> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TResponse> next,
        CancellationToken cancellationToken)
    {
        var messageType = typeof(TMessage).Name;
        
        // ReSharper disable once SuspiciousTypeConversion.Global
        if (message is IUserRequestBase userRequest)
        {
            logger.LogInformation(
                "Handling {MessageType} for User {UserId} with content: {@Message}",
                messageType, userRequest.UserId, message);
        }
        else
        {
            logger.LogInformation(
                "Handling {MessageType} with content: {@Message}",
                messageType, message);
        }

        var sw = Stopwatch.StartNew();

        try
        {
            var response = await next(message, cancellationToken);
            sw.Stop();

            logger.LogInformation(
                "Completed {MessageType} in {Elapsed}ms",
                messageType, sw.ElapsedMilliseconds);

            return response;
        }
        catch (Exception ex)
        {
            sw.Stop();

            logger.LogError(ex,
                "Error handling {MessageType} after {Elapsed}ms: {ErrorMessage}",
                messageType, sw.ElapsedMilliseconds, ex.Message);

            throw;
        }
    }
}
