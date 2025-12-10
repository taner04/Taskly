using System.Security.Claims;

namespace Api.Features.Users.Services;

public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor)
{
    private HttpContext HttpContext => httpContextAccessor.HttpContext
        ?? throw new InvalidOperationException("HTTP context is not available.");
    
    private ClaimsPrincipal User => HttpContext.User;
    
    private ClaimsPrincipal GetCurrentUser()
    {
        return User.Identity?.IsAuthenticated != true
            ? throw new UnauthorizedAccessException("User is not authenticated.")
            : User;
    }

    public string GetCurrentUserId()
    {
        var userId = 
            GetCurrentUser().FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException("user id is missing.");

        return userId;  
    }
}