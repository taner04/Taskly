using FluentAssertions;
using Newtonsoft.Json.Linq;

namespace IntegrationTests.Extensions;

public static class HttpResponseMessageExtension
{
    private static async Task<JToken> ReadAsJToken(
        this HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(content))
            return JValue.CreateNull();

        return JToken.Parse(content);
    }

    public static async Task<T> MapTo<T>(
        this HttpResponseMessage response,
        CancellationToken cancellationToken = default)
        where T : class
    {
        var token = await response.ReadAsJToken(cancellationToken);

        // Case 1: { "value": ... }
        if (token is JObject obj && obj["value"] is not null) return obj["value"]!.ToObject<T>()!;

        // Case 2: raw object or array
        return token.ToObject<T>()!;
    }

    public static async Task ContainsErrorCode(
        this HttpResponseMessage response,
        string errorCode,
        CancellationToken cancellationToken = default)
    {
        var token = await response.ReadAsJToken(cancellationToken);
        var actual = token["errorCode"]?.Value<string>();

        actual.Should().NotBeNull("the response should contain an 'errorCode' field");
        actual.Should().BeEquivalentTo(errorCode, "error codes should match ignoring case");
    }
}