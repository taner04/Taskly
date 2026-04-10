namespace Taskly.WebApi.Client.Abstractions;

public interface IApiHttpClient
{
    IApiClient Client { get; }

    void SetAccessToken(string accessToken);
    void ClearAccessToken();
}