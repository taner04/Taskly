namespace Api.Features.Shared.Exceptions;

public sealed class ModelNotFoundException<T>(Guid id) :
    ModelBaseException(
        $"Could not find {typeof(T).Name.ToLower()}.",
        $"{typeof(T).Name} with ID '{id}' was not found.",
        $"{typeof(T).Name}.NotFound",
        HttpStatusCode.NotFound);