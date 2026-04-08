namespace Taskly.Desktop.Common.Infrastructure.Auth0.EventArgs;

public class SignInEventArgs(string accessToken) : System.EventArgs
{
    public string AccessToken { get; } = accessToken;
}
