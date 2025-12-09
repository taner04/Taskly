using FluentAssertions;
using Newtonsoft.Json.Linq;

namespace IntegrationTests.Extensions;

public static class HttpResponseMessageExtension
{
    extension(HttpResponseMessage response)
    {
        private async Task<JToken> ReadAsJToken(
            CancellationToken cancellationToken)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            return string.IsNullOrWhiteSpace(content) ? JValue.CreateNull() : JToken.Parse(content);
        }

        public async Task<T> MapTo<T>(
            CancellationToken cancellationToken = default)
            where T : class
        {
            var token = await response.ReadAsJToken(cancellationToken);

            // Case 1: { "value": ... }
            if (token is JObject obj && obj["value"] is not null)
            {
                return obj["value"]!.ToObject<T>()!;
            }

            // Case 2: raw object or array
            return token.ToObject<T>()!;
        }

        public async Task ContainsErrorCode(
            string errorCode,
            CancellationToken cancellationToken = default)
        {
            var token = await response.ReadAsJToken(cancellationToken);
            var actual = token["errorCode"]?.Value<string>();

            actual.Should().NotBeNull("the response should contain an 'errorCode' field");
            actual.Should().BeEquivalentTo(errorCode, "error codes should match ignoring case");
        }   
    }
}