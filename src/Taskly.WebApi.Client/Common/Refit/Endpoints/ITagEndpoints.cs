using Taskly.WebApi.Client.Common.Dtos.Tags;

namespace Taskly.WebApi.Client.Common.Refit.Endpoints;

public interface ITagEndpoints
{
    [Post(ApiRoutes.Tags.Create)]
    Task<HttpResponseMessage> CreateTagAsync(
        [Body] CreateTagRequest command,
        CancellationToken cancellationToken = default);

    [Delete(ApiRoutes.Tags.Remove)]
    Task<HttpResponseMessage> DeleteTagAsync(
        Guid tagId,
        CancellationToken cancellationToken = default);

    [Get(ApiRoutes.Tags.GetTags)]
    Task<HttpResponseMessage> GetTagsAsync(
        PaginationQuery query,
        CancellationToken cancellationToken = default);

    [Put(ApiRoutes.Tags.Update)]
    Task<HttpResponseMessage> UpdateTagAsync(
        Guid tagId,
        [Body] UpdateTagRequest body,
        CancellationToken cancellationToken = default);
}