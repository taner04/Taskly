// ReSharper disable CheckNamespace

using Api.Features.Shared;
using Api.Features.Tags.Endpoints;
using Api.Features.Tags.Model;
using Refit;

namespace Api.Client;

public partial interface IApiClient
{
    [Post(Routes.Tags.Create)]
    Task<HttpResponseMessage> CreateTagAsync(
        CreateTag.Command command,
        CancellationToken cancellationToken = default);

    [Delete(Routes.Tags.Remove)]
    Task<HttpResponseMessage> DeleteTagAsync(
        TagId tagId,
        CancellationToken cancellationToken = default);

    [Get(Routes.Tags.GetTags)]
    Task<HttpResponseMessage> GetTagsAsync(
        GetTags.Query query,
        CancellationToken cancellationToken = default);

    [Put(Routes.Tags.Update)]
    Task<HttpResponseMessage> UpdateTagAsync(
        TagId tagId,
        [Body] UpdateTag.Command.CommandBody body,
        CancellationToken cancellationToken = default);
}