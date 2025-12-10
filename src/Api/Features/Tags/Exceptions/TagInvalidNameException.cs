namespace Api.Features.Tags.Exceptions;

public sealed class TagInvalidNameException(int currentLength) :
    ModelBaseException(
        "The tag name is invalid.",
        $"The tag name length of {currentLength} exceeds the maximum allowed length between {Tag.MinNameLength} and {Tag.MaxNameLength} characters.",
        "Tag.InvalidName",
        HttpStatusCode.BadRequest);