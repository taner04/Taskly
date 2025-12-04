// ReSharper disable CheckNamespace

using Api.Features.Shared.Dtos.Tags;
using Api.Features.Tags.Endpoints;
using Refit;

namespace Api;

public partial interface IApiClient
{
    [Post(Routes.Tags.Create)]
    Task<ApiResponse<CreateTag.Response>> CreateTagAsync(
        CreateTag.Command command,
        CancellationToken cancellationToken = default);

    [Delete(Routes.Tags.Delete)]
    Task<HttpResponseMessage> DeleteTagAsync(
        TagId tagId,
        CancellationToken cancellationToken = default);

    [Get(Routes.Tags.GetTags)]
    Task<ApiResponse<List<TagDto>>> GetTagsAsync(CancellationToken cancellationToken = default);

    [Put(Routes.Tags.Update)]
    Task<HttpResponseMessage> UpdateTagAsync(
        TagId tagId,
        [Body] UpdateTag.Command.CommandBody body,
        CancellationToken cancellationToken = default);
}