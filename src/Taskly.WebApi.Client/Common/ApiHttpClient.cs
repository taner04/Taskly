using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Refit;
using Taskly.WebApi.Client.Abstractions;

namespace Taskly.WebApi.Client.Common;

internal sealed class ApiHttpClient : IApiHttpClient
{
    private readonly HttpClient _httpClient;

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    internal ApiHttpClient(ApiHttpClientOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        _httpClient = new HttpClient
        {
            BaseAddress = options.BaseAddress,
            Timeout = options.Timeout,
        };

        Client = RestService.For<IApiClient>(_httpClient,
            new RefitSettings(new SystemTextJsonContentSerializer(_jsonOptions)));
    }

    internal
        ApiHttpClient( //For integration testing, allows for more control over the HttpClient configuration
            HttpClient httpClient) //TODO: Find a better way
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        _httpClient = httpClient;

        Client = RestService.For<IApiClient>(_httpClient);
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
