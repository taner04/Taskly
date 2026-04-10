namespace Taskly.WebApi.Common.Shared.Exceptions;

internal sealed class ModelNotFoundException<T>(Guid id) :
    TasklyException(
        $"Could not find {typeof(T).Name.ToLower()}.",
        $"{typeof(T).Name} with ID '{id}' was not found.",
        $"{typeof(T).Name}.NotFound",
        HttpStatusCode.NotFound)
{
}