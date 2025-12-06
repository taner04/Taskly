namespace Api.Features.Tags.Exceptions;

public sealed class TagAlreadyExistsException(string tagName) :
    ModelBaseException(
        "Tag already exists.",
        $"A tag with the name '{tagName}' already exists.",
        "Tag.AlreadyExists",
        HttpStatusCode.Conflict);