namespace Taskly.WebApi.Features.Tags.Exceptions;

internal sealed class InvalidTagNameException : TasklyException
{
    private InvalidTagNameException(
        string name,
        string description)
        : base(
            "Invalid Tag Name",
            description,
            "Tag.InvalidName",
            HttpStatusCode.BadRequest)
    {
    }

    public static void ThrowIfInvalid(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidTagNameException(
                name,
                "Tag name cannot be null, empty, or consist only of whitespace.");
        }

        if (name.Length is > Tag.MaxNameLength or < Tag.MinNameLength)
        {
            throw new InvalidTagNameException(
                name,
                $"Tag name must be between {Tag.MinNameLength} and {Tag.MaxNameLength} characters long. " +
                $"Provided name length: {name.Length}.");
        }
    }
}