namespace Taskly.WebApi.Client.Results;

public class WebClientResult
{
    private readonly WebClientError? _error;

    protected WebClientResult(WebClientError? error)
    {
        _error = error;
    }

    public bool IsSuccess => _error is null;

    public WebClientError Error => _error ??
                                   throw new InvalidOperationException(
                                       "Cannot access ProblemDetails when the response is successful.");

    public static implicit operator WebClientResult(WebClientError error) => new(error);

    public static WebClientResult Success() => new(null);

    public static WebClientResult Failure(WebClientError error) => new(error);
}