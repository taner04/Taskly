using Api.Features.Shared.Exceptions;

namespace Api.Features.Tags.Exceptions;

public sealed class TagAlreadyExistsException(string tagName) :
    ApiException(
        "Tag already exists.",
        $"A tag with the name '{tagName}' already exists.",
        "Tag.AlreadyExists");