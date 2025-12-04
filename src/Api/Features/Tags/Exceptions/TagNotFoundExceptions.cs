namespace Api.Features.Tags.Exceptions;

public sealed class TagNotFoundExceptions(TagId tagId) :
    ApiException(
        "CouldNot find tag.",
        $"Tag with ID '{tagId}' was not found.",
        "Tag.NotFound");