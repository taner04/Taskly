using System.Security.Claims;
using Api.Features.Users.Model;

namespace Api.Features.Users.Services;

public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor)
{
    private HttpContext HttpContext => httpContextAccessor.HttpContext
                                       ?? throw new InvalidOperationException("HTTP context is not available.");

    private ClaimsPrincipal User => HttpContext.User;

    public string GetAuth0Id()
    {
        return GetClaimValue<string>(ClaimTypes.NameIdentifier);
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

    public T GetClaimValue<T>(
        string claimType)
    {
        var claimValue =
            User.FindFirst(claimType)?.Value ??
            throw new UnauthorizedAccessException($"Claim '{claimType}' is missing.");

        return (T)Convert.ChangeType(claimValue, typeof(T));
    }
}