using FluentAssertions;
using Newtonsoft.Json.Linq;

namespace IntegrationTests.Extensions;

public static class HttpResponseMessageExtension
{
    public static async Task<T> MapTo<T>(this HttpResponseMessage response, CancellationToken cancellationToken = default) where T : class
    {
        var obj = JObject.Parse(await response.Content.ReadAsStringAsync(cancellationToken))["value"]?.ToObject<T>();
        obj.Should().NotBeNull();
        
        return obj;
    }
    
    public static async Task ContainsErrorCode(this HttpResponseMessage response, string errorCode, CancellationToken cancellationToken = default)
    {
        var value = JObject.Parse(await response.Content.ReadAsStringAsync(cancellationToken))["errorCode"]?.Value<string>();
        value.Should().NotBeNull();
        
        value.ToLower().Should().Be(errorCode.ToLower());
    }
}