namespace Api.Features.Tags.Exceptions;

public sealed class TagsNotFoundExceptions(List<TagId> tagIds) :
    ModelBaseException(
        "Could not find one or more tags.",
        $"Tags with following IDs not found: '{string.Join(", ", tagIds)}'",
        "Tag.NotFound",
        HttpStatusCode.NotFound);