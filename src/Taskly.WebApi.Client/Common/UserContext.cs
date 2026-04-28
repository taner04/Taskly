using System.Security.Claims;

namespace Taskly.WebApi.Client.Common;

internal sealed class UserContext : IUserContext
{
    private ClaimsPrincipal ClaimsPrincipal { get; set; } = null!;
    public string AccessToken { get; private set; } = null!;
    public DateTimeOffset AccessTokenExpiration { get; private set; }

    public T GetClaim<T>(string claimType)
    {
        ArgumentException.ThrowIfNullOrEmpty(claimType);

        var claim = ClaimsPrincipal.FindFirst(claimType) ?? throw new InvalidOperationException("Claim not found.");
        return (T)Convert.ChangeType(claim.Value, typeof(T));
    }

    public void InitUserContext(
        string accessToken,
        DateTimeOffset accessTokenExpiration,
        ClaimsPrincipal claimsPrincipal)
    {
        AccessToken = accessToken;
        AccessTokenExpiration = accessTokenExpiration;
        ClaimsPrincipal = claimsPrincipal;
    }

    public void ClearContext()
    {
        AccessToken = null!;
        ClaimsPrincipal = null!;
        AccessTokenExpiration = DateTimeOffset.MinValue;
    }
}