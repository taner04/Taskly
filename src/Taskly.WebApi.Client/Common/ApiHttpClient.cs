using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Taskly.WebApi.Client.Common;

internal sealed class ApiHttpClient : IApiHttpClient
{
    private static readonly RefitSettings RefitSettings = new(new SystemTextJsonContentSerializer(
        new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        }));

    private readonly HttpClient _httpClient;

    private ApiHttpClient(HttpClient httpClient, RefitSettings refitSettings)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        _httpClient = httpClient;

        Client = RestService.For<IApiClient>(_httpClient, refitSettings);
    }

    internal ApiHttpClient(ApiHttpClientOptions options) :
        this(new HttpClient { BaseAddress = options.BaseAddress, Timeout = options.Timeout }, RefitSettings)
    {
    }

    // For integration tests
    internal ApiHttpClient(HttpClient httpClient) : this(httpClient, RefitSettings)
    {
    }

    public IApiClient Client { get; }

    public void SetAccessToken(string accessToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(accessToken);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
    }

    public void ClearAccessToken()
    {
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }
}