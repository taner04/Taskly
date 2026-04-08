namespace Taskly.WebApi.Features.Tags.Exceptions;

internal sealed class TagAlreadyExistsException(string tagName) :
    ModelBaseException(
        "Tag already exists.",
        $"A tag with the name '{tagName}' already exists.",
        "Tag.AlreadyExists",
        HttpStatusCode.Conflict);
