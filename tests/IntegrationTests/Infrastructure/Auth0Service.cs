using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Api.Composition.Options;

namespace IntegrationTests.Infrastructure;

public sealed class Auth0Service(Auth0Options auth0Options)
{
    public async Task<string> GetAccessTokenAsync()
    {
        var uri = $"https://{auth0Options.Domain}/oauth/token";

        using var httpClient = new HttpClient();
        
        var tokenRequest = new TokenRequest
        {
            ClientId = auth0Options.ClientId,
            ClientSecret = auth0Options.ClientSecret,
            Audience = auth0Options.Audience,
            GrantType = "client_credentials"
        };

        var content = new StringContent(
            JsonSerializer.Serialize(tokenRequest),
            Encoding.UTF8,
            "application/json"
        );

        var response = await httpClient.PostAsync(uri, content);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException("Failed to obtain access token from Auth0.");
        }

        var json = await response.Content.ReadAsStringAsync();

        var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(json);

        return string.IsNullOrWhiteSpace(tokenResponse?.AccessToken) ? 
            throw new InvalidOperationException("Auth0 response does not contain access_token.") : 
            tokenResponse.AccessToken;
    }

    private record TokenRequest
    {
        [JsonPropertyName("client_id")] public string ClientId { get; init; } = null!;

        [JsonPropertyName("client_secret")] public string ClientSecret { get; init; } = null!;

        [JsonPropertyName("audience")] public string Audience { get; init; } = null!;

        [JsonPropertyName("grant_type")] public string GrantType { get; init; } = null!;
    }

    private record TokenResponse
    {
        [JsonPropertyName("access_token")] public string AccessToken { get; init; } = null!;

        [JsonPropertyName("token_type")] public string TokenType { get; init; } = null!;
    }
}