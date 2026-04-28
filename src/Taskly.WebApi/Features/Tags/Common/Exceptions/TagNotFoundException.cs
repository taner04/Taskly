using TagId = Taskly.WebApi.Features.Tags.Common.Models.TagId;

namespace Taskly.WebApi.Features.Tags.Common.Exceptions;

internal sealed class TagNotFoundException(List<TagId> tagIds) :
    TasklyException(
        "Could not find one or more tags.",
        $"Tags with following IDs not found: '{string.Join(", ", tagIds)}'",
        "Tag.NotFound",
        HttpStatusCode.NotFound);