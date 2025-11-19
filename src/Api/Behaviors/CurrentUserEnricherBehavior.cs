using System.Security.Claims;

namespace Api.Behaviors;

public sealed class CurrentUserEnricherBehavior<TMessage, TResponse>(IHttpContextAccessor accessor)
    : Behavior<TMessage, TResponse>
    where TMessage : IUserRequestBase
{
    public override async ValueTask<TResponse> HandleAsync(TMessage request, CancellationToken cancellationToken)
    {
        var userId = accessor.HttpContext?.User.Claims
            .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            throw new UnauthorizedAccessException("The user is not authenticated.");
        }

        request.UserId = userId;
        return await Next(request, cancellationToken);
    }
}