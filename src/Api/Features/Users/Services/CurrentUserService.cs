using System.Security.Claims;
using Api.Features.Users.Model;

namespace Api.Features.Users.Services;

public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor)
{
    private HttpContext HttpContext => httpContextAccessor.HttpContext
        ?? throw new InvalidOperationException("HTTP context is not available.");
    
    private ClaimsPrincipal User => HttpContext.User;
    
    public ClaimsPrincipal GetCurrentUser()
    {
        return User.Identity?.IsAuthenticated != true
            ? throw new UnauthorizedAccessException("User is not authenticated.")
            : User;
    }

    public string GetAuth0Id()
    {
        var userId = 
            GetCurrentUser().FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException("user id is missing.");
        
        return userId;
    }
    
    public UserId GetUserId()
    {
        if (httpContextAccessor.HttpContext!.Items.TryGetValue("UserId", out var id) 
            && id is UserId userId)
        {
            return userId;
        }

        throw new UnauthorizedAccessException("User not provisioned.");
    }

    
    public T GetClaimValue<T>(string claimType)
    {
        var claimValue = 
            GetCurrentUser().FindFirst(claimType)?.Value ?? throw new UnauthorizedAccessException($"Claim '{claimType}' is missing.");

        return (T)Convert.ChangeType(claimValue, typeof(T));
    }
}