using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Refit;

namespace Api.Client;

public class ApiHttpClient
{
    private readonly HttpClient _httpClient;

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    public ApiHttpClient(string baseAddress)
    {
        ArgumentException.ThrowIfNullOrEmpty(baseAddress);

        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(baseAddress)
        };

        Client = RestService.For<IApiClient>(_httpClient,
            new RefitSettings(new SystemTextJsonContentSerializer(_jsonOptions)));
    }

    public
        ApiHttpClient(
            HttpClient httpClient) //For integration testing, allows for more control over the HttpClient configuration
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