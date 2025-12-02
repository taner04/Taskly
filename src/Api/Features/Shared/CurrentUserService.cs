using System.Security.Claims;

namespace Api.Features.Shared;

public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor)
{
    public ClaimsPrincipal GetCurrentUser()
    {
        var user = httpContextAccessor.HttpContext?.User;

        if (user is null || !user.Identity?.IsAuthenticated == true)
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        return user;
    }

    public string GetCurrentUserId()
    {
        var user = GetCurrentUser();

        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return string.IsNullOrEmpty(userId)
            ? throw new UnauthorizedAccessException("UserId claim is missing.")
            : userId;
    }
}