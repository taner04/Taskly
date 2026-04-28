using System.Net.Http.Headers;

namespace Taskly.WebApi.Client.Common.Refit.DelegatingHandlers;

internal sealed class BearerDelegatingHandler(IUserContext userContext) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(userContext);
        if (userContext.AccessTokenExpiration < DateTimeOffset.UtcNow)
        {
            //potentially refresh token here if it has expired etc.
            throw new InvalidOperationException("Access token has expired. Implement token refresh logic here.");
        }

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userContext.AccessToken);
        return await base.SendAsync(request, cancellationToken);
    }
}