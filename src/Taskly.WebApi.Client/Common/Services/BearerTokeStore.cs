namespace Taskly.WebApi.Client.Common.Services;

public sealed class BearerTokeStore
{
    private string? AccessToken { get; set; }
    
    internal string GetAccessToken()
    {
        ArgumentNullException.ThrowIfNull(AccessToken);   
        return AccessToken;
    }
    
    internal void SetAccessToken(string accessToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(accessToken);
        AccessToken = accessToken;
    }
    
    internal void ClearToken(string refreshToken) => AccessToken = null;
}