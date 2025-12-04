// ReSharper disable CheckNamespace

using Api.Features.Tags.Endpoints;
using Api.Shared.Dtos.Tags;
using Refit;

namespace Api;

public partial interface IApiClient
{
    [Post(Routes.Tags.Create)]
    Task<CreateTag.Dto> CreateTagAsync(
        CreateTag.Command command,
        CancellationToken cancellationToken = default);

    [Delete(Routes.Tags.Delete)]
    Task DeleteTagAsync(
        TagId tagId,
        CancellationToken cancellationToken = default);

    [Get(Routes.Tags.GetTags)]
    Task<List<TagDto>> GetTagsAsync(CancellationToken cancellationToken = default);

    [Put(Routes.Tags.Update)]
    Task UpdateTagAsync(
        TagId tagId,
        [Body] UpdateTag.Command.CommandBody body,
        CancellationToken cancellationToken = default);
}