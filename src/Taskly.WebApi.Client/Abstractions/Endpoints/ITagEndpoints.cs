using Taskly.WebApi.Features.Tags.Endpoints;

namespace Taskly.WebApi.Client.Abstractions.Endpoints;

public interface ITagEndpoints
{
    [Post(ApiRoutes.Tags.Create)]
    Task<HttpResponseMessage> CreateTagAsync(
        [Body] CreateTag.Command command,
        CancellationToken cancellationToken = default);

    [Delete(ApiRoutes.Tags.Remove)]
    Task<HttpResponseMessage> DeleteTagAsync(
        TagId tagId,
        CancellationToken cancellationToken = default);

    [Get(ApiRoutes.Tags.GetTags)]
    Task<HttpResponseMessage> GetTagsAsync(
        GetTags.Query query,
        CancellationToken cancellationToken = default);

    [Put(ApiRoutes.Tags.Update)]
    Task<HttpResponseMessage> UpdateTagAsync(
        TagId tagId,
        [Body] UpdateTag.Command.CommandBody body,
        CancellationToken cancellationToken = default);
}