using Taskly.WebApi.Common;

namespace Taskly.WebApi.Client.Common.Extensions;

internal static class HttpResponseMessageExtensions
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    extension(HttpResponseMessage httpResponseMessage)
    {
        internal async Task<T> DeserializeTo<T>(CancellationToken cancellationToken) where T : class
        {
            ArgumentNullException.ThrowIfNull(httpResponseMessage);
            ArgumentNullException.ThrowIfNull(httpResponseMessage.Content);

            var contentString =
                await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

            var obj = JsonSerializer.Deserialize<T>(contentString, JsonSerializerOptions);
            ArgumentNullException.ThrowIfNull(obj);

            return obj;
        }

        internal async Task<WebClientError> DeserializeToWebClientError(CancellationToken cancellationToken)
        {
            var problemDetails = await httpResponseMessage.DeserializeTo<WebApiProblemDetails>(cancellationToken);
            return WebClientError.FromProblemdetails(problemDetails);
        }
    }
}