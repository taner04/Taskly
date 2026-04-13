using System.Security.Claims;

namespace Taskly.WebApi.Features.Users.Services;

public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor)
{
    public const string SubClaim = "sub";
    public const string RoleClaim = "permissions";

    private HttpContext HttpContext => httpContextAccessor.HttpContext
                                       ?? throw new InvalidOperationException("HTTP context is not available.");

    private ClaimsPrincipal User => HttpContext.User;

    internal string GetAuth0Id()
    {
        var auth0Id = GetClaimValue<string>(SubClaim);
        return string.IsNullOrEmpty(auth0Id)
            ? throw new UnauthorizedAccessException("Auth0 ID claim is missing or empty.")
            : auth0Id;
    }

    internal UserId GetUserId()
    {
        if (httpContextAccessor.HttpContext!.Items.TryGetValue("UserId", out var id)
            && id is UserId userId)
        {
            return userId;
        }

        throw new UnauthorizedAccessException("User not provisioned.");
    }

    internal T GetClaimValue<T>(
        string claimType)
    {
        var claimValue =
            User.FindFirst(claimType)?.Value ??
            throw new UnauthorizedAccessException($"Claim '{claimType}' is missing.");

        return (T)Convert.ChangeType(claimValue, typeof(T));
    }
}