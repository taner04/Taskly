using Microsoft.Extensions.Configuration;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace IntegrationTests.Common
{
    public sealed class Auth0Service(IConfiguration configuration)
    {
        public string GetAccessToken()
        {
            var uri = $"https://{configuration["Auth0:Domain"]}/oauth/token";
            using var restSharpclient = new RestClient(uri);
            var request = new RestRequest("", Method.Post);
            request.AddHeader("content-type", "application/json");

            var tokenRequestAsJson = JsonSerializer.Serialize(new TokenRequest
            {
                ClientId = configuration["Auth0:Client_Id"]!,
                ClientSecret = configuration["Auth0:Client_Secret"]!,
                Audience = configuration["Auth0:Audience"]!,
                GrantType = configuration["Auth0:Grant_Type"]!
            });

            request.AddParameter("application/json", tokenRequestAsJson, ParameterType.RequestBody);
            var response = restSharpclient.Execute(request);

            if (!response.IsSuccessful || response.Content is null)
            {
                throw new InvalidOperationException("Failed to obtain access token from Auth0.");
            }

            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(response.Content);

            if (tokenResponse?.AccessToken is null || string.IsNullOrWhiteSpace(tokenResponse.AccessToken))
            {
                throw new InvalidOperationException("Auth0 response does not contain access_token.");
            }

            return tokenResponse.AccessToken;
        }

        private class TokenRequest
        {
            [JsonPropertyName("client_id")]
            public string ClientId { get; set; } = null!;
            [JsonPropertyName("client_secret")]
            public string ClientSecret { get; set; } = null!;
            [JsonPropertyName("audience")]
            public string Audience { get; set; } = null!;
            [JsonPropertyName("grant_type")]
            public string GrantType { get; set; } = null!;
        }

        private class TokenResponse
        {
            [JsonPropertyName("access_token")]
            public string AccessToken { get; set; } = null!;
            [JsonPropertyName("token_type")]
            public string TokenType { get; set; } = null!;
        }
    }
}
