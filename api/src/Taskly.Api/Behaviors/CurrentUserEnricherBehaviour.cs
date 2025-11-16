using System.Security.Claims;
using Mediator;
using Taskly.Api.Abstractions;

namespace Taskly.Api.Behaviors;

public sealed class CurrentUserEnricherBehaviour<TMessage, TResponse>(IHttpContextAccessor accessor)
    : IPipelineBehavior<TMessage, TResponse>
    where TMessage : IUserRequestBase, IMessage
{
    public async ValueTask<TResponse> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TResponse> next,
        CancellationToken cancellationToken)
    {
        var userId = accessor.HttpContext?.User.Claims
            .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            throw new UnauthorizedAccessException("The user is not authenticated.");
        }
        
        message.UserId = userId;
        return await next(message, cancellationToken);
    }
}