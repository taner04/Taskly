using Taskly.WebApi.Client.Common.Shared.Extensions;

namespace Taskly.WebApi.Client.Common.Shared;

public abstract class EndpointService
{
    private static WebClientError ApiExceptionError(string message) =>
        WebClientError.CustomError("ApiError", "An error occurred while calling the API", message);

    private static WebClientError ExceptionError(string message) =>
        WebClientError.CustomError("UnexpectedError", "An unexpected error occurred", message);

    protected static async Task<WebClientResult> HttpCallOrchestrator(
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

    protected static async Task<WebClientResult<TResult>> HttpCallOrchestrator<TResult>(
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