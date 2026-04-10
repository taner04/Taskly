using Taskly.WebApi.Common.Shared.Exceptions;

namespace Taskly.WebApi.Features.Tags.Exceptions;

internal sealed class TagAlreadyExistsException(string tagName) :
    TasklyException(
        "Tag already exists.",
        $"A tag with the name '{tagName}' already exists.",
        "Tag.AlreadyExists",
        HttpStatusCode.Conflict);