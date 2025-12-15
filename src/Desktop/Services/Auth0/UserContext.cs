using Duende.IdentityModel.OidcClient;

namespace Desktop.Services.Auth0;

public sealed class UserContext
{
    public string Email { get; init; } = null!;
    public string AccessToken { get; init; } = null!;
    public string IdentityToken { get; init; } = null!;

    public IReadOnlyCollection<string> Roles { get; init; } = [];

    public static UserContext FromLoginResult(
        LoginResult result)
    {
        var user = result.User;

        return new UserContext
        {
            Email = user.FindFirst("email")?.Value ?? "",
            AccessToken = result.AccessToken,
            IdentityToken = result.IdentityToken,
            Roles = user.FindAll("https://taskly-api/roles").Select(c => c.Value).ToList()
        };
    }
}