using Taskly.WebApi.Client.Common.Extensions;

namespace Taskly.WebApi.Client.Common.Helper;

internal static class HttpOrchestrator
{
    private static WebClientError ApiExceptionError(string message) =>
        WebClientError.CustomError("ApiError", "An error occurred while calling the API", message);

    private static WebClientError ExceptionError(string message) =>
        WebClientError.CustomError("UnexpectedError", "An unexpected error occurred", message);
    
    internal static async Task<WebClientResult> OrchestrateAsync(
        Func<Task<HttpResponseMessage>> apiCall,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await apiCall();
            return response.IsSuccessStatusCode
                ? WebClientResult.Success()
                : await response.DeserializeToWebClientError(cancellationToken);
        }
        catch (Exception ex)
        {
            return ex switch
            {
                ApiException apiException => ApiExceptionError(apiException.Message),
                _ => ExceptionError(ex.Message)
            };
        }
    }

    internal static async Task<WebClientResult<TResult>> OrchestrateAsync<TResult>(
        Func<Task<HttpResponseMessage>> apiCall,
        CancellationToken cancellationToken) where TResult : class
    {
        try
        {
            var response = await apiCall();
            return response.IsSuccessStatusCode
                ? await response.DeserializeTo<TResult>(cancellationToken)
                : await response.DeserializeToWebClientError(cancellationToken);
        }
        catch (Exception ex)
        {
            return ex switch
            {
                ApiException apiException => ApiExceptionError(apiException.Message),
                _ => ExceptionError(ex.Message)
            };
        }
    }
}