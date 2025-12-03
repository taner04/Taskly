using System.Security.Claims;

namespace Api.Features.Users;

public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor)
{
    private ClaimsPrincipal GetCurrentUser()
    {
        var user = httpContextAccessor.HttpContext?.User;

        return user?.Identity?.IsAuthenticated != true
            ? throw new UnauthorizedAccessException("User is not authenticated.")
            : user;
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