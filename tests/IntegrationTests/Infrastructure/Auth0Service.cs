using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IntegrationTests.Infrastructure;

public sealed class Auth0Service(IConfiguration configuration)
{
    public async Task<string> GetAccessTokenAsync()
    {
        var uri = $"https://{configuration["Auth0:Domain"]}/oauth/token";

        using var httpClient = new HttpClient();

        var tokenRequest = new TokenRequest
        {
            ClientId = configuration["Auth0:Client_Id"]!,
            ClientSecret = configuration["Auth0:Client_Secret"]!,
            Audience = configuration["Auth0:Audience"]!,
            GrantType = configuration["Auth0:Grant_Type"]!
        };

        var content = new StringContent(
            JsonSerializer.Serialize(tokenRequest),
            Encoding.UTF8,
            "application/json"
        );

        var response = await httpClient.PostAsync(uri, content);

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException("Failed to obtain access token from Auth0.");

        var json = await response.Content.ReadAsStringAsync();

        var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(json);

        if (string.IsNullOrWhiteSpace(tokenResponse?.AccessToken))
            throw new InvalidOperationException("Auth0 response does not contain access_token.");

        return tokenResponse.AccessToken;
    }

    private class TokenRequest
    {
        [JsonPropertyName("client_id")] public string ClientId { get; set; } = null!;

        [JsonPropertyName("client_secret")] public string ClientSecret { get; set; } = null!;

        [JsonPropertyName("audience")] public string Audience { get; set; } = null!;

        [JsonPropertyName("grant_type")] public string GrantType { get; set; } = null!;
    }

    private class TokenResponse
    {
        [JsonPropertyName("access_token")] public string AccessToken { get; set; } = null!;

        [JsonPropertyName("token_type")] public string TokenType { get; set; } = null!;
    }
}