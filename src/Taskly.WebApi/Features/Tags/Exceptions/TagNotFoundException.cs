using TagId = Taskly.WebApi.Features.Tags.Models.TagId;

namespace Taskly.WebApi.Features.Tags.Exceptions;

internal sealed class TagNotFoundException(List<TagId> tagIds) :
    TasklyException(
        "Could not find one or more tags.",
        $"Tags with following IDs not found: '{string.Join(", ", tagIds)}'",
        "Tag.NotFound",
        HttpStatusCode.NotFound);