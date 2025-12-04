namespace Api.Shared.Exceptions;

public abstract class ApiException(
    string title,
    string message,
    string errorCode) : Exception(message)
{
    public string Title { get; init; } = title;
    public string ErrorCode { get; init; } = errorCode;
}