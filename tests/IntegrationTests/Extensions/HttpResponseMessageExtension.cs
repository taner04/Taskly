using FluentAssertions;
using Newtonsoft.Json.Linq;

namespace IntegrationTests.Extensions;

public static class HttpResponseMessageExtension
{
    private static async Task<JObject> ReadAsJObject(
        this HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return JObject.Parse(content);
    }

    public static async Task<T> MapTo<T>(
        this HttpResponseMessage response,
        CancellationToken cancellationToken = default)
        where T : class
    {
        var json = await response.ReadAsJObject(cancellationToken);
        var obj = json["value"]?.ToObject<T>();

        obj.Should().NotBeNull("the response should contain a 'value' object that maps to {0}", typeof(T).Name);

        return obj!;
    }

    public static async Task ContainsErrorCode(
        this HttpResponseMessage response,
        string errorCode,
        CancellationToken cancellationToken = default)
    {
        var json = await response.ReadAsJObject(cancellationToken);
        var actual = json["errorCode"]?.Value<string>();

        actual.Should().NotBeNull("the response should contain an 'errorCode' field");
        actual.Should().BeEquivalentTo(errorCode, "error codes should match ignoring case");
    }
}