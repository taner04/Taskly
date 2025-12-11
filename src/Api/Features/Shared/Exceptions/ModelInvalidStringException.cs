namespace Api.Features.Shared.Exceptions;

public sealed class ModelInvalidStringException<T>(
    string propertyName,
    int currentLength,
    int minLength,
    int maxLength)
    : ModelBaseException(
        $"The {typeof(T).Name.ToLower()} {propertyName.ToLower()} is invalid.",
        $"The {typeof(T).Name.ToLower()} {propertyName.ToLower()} length of {currentLength} is outside the allowed range of {minLength} to {maxLength} characters.",
        $"{typeof(T).Name}.Invalid{propertyName}",
        HttpStatusCode.BadRequest);