using System.Security.Claims;

namespace Taskly.WebApi.Client.Common.Abstractions;

public interface IUserContext
{
    string AccessToken { get; }
    DateTimeOffset AccessTokenExpiration { get; }

    void InitUserContext(string accessToken, DateTimeOffset accessTokenExpiration, ClaimsPrincipal claimsPrincipal);
    void ClearContext();
    T GetClaim<T>(string claimType);
}