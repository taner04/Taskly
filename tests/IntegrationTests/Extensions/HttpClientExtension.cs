using System.IdentityModel.Tokens.Jwt;

namespace IntegrationTests.Extensions;

public static class HttpClientExtension
{
    public static string GetUserId(
        this HttpClient client)
    {
        var token = client.DefaultRequestHeaders.Authorization?.Parameter
                    ?? throw new InvalidOperationException("No JWT token found.");

        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
        var claimValue = securityToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value ??
                         throw new InvalidOperationException();

        return claimValue ?? throw new InvalidOperationException("User ID claim not found in token.");
    }
}