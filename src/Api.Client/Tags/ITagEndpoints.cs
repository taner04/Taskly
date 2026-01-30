using Api.Features.Shared;
using Api.Features.Tags.Endpoints;
using Api.Features.Tags.Model;
using Refit;

namespace Api.Client.Tags;

public interface ITagEndpoints
{
    [Post(ApiRoutes.Tags.Create)]
    Task<HttpResponseMessage> CreateTagAsync(
    CreateTag.Command command,
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
