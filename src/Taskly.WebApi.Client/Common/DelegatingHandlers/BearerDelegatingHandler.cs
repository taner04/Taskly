using System.Net.Http.Headers;
using Taskly.WebApi.Client.Common.Services;

namespace Taskly.WebApi.Client.Common.DelegatingHandlers;

internal sealed class BearerDelegatingHandler(BearerTokeStore bearerTokeStore) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        //potentially refresh token here if it has expired etc.

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerTokeStore.GetAccessToken());
        return await base.SendAsync(request, cancellationToken);
    }
}