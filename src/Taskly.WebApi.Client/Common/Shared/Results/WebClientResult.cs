namespace Taskly.WebApi.Client.Common.Shared.Results;

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

public sealed class WebClientResult<T> : WebClientResult
{
    private WebClientResult(T value) : base(null!)
    {
        Value = value;
    }

    private WebClientResult(WebClientError error) : base(error)
    {
    }


    public T Value => field ??
                      throw new InvalidOperationException("Cannot access Value when the response is not successful.");

    public static implicit operator WebClientResult<T>(T value) => new(value);
    public static implicit operator WebClientResult<T>(WebClientError error) => new(error);

    public static WebClientResult<T> Success(T value) => new(value);
}