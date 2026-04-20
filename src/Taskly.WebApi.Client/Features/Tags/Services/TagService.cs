using Taskly.Shared.Pagination;
using Taskly.WebApi.Client.Features.Tags.Dtos;
using Taskly.WebApi.Features.Tags.Endpoints;

namespace Taskly.WebApi.Client.Features.Tags.Services;

public sealed class TagService(IRefitWebApiClient webApiHttpClient) : EndpointService
{
    public async Task<WebClientResult> CreateTagAsync(
        CreateTagRequest request,
        CancellationToken cancellationToken)
    {
        return await HttpCallOrchestrator(() => webApiHttpClient.CreateTagAsync(request, cancellationToken),
            cancellationToken);
    }

    public async Task<WebClientResult<PaginationResult<GetTags.Response>>> GetTagsAsync(
        PaginationQuery query,
        CancellationToken cancellationToken)
    {
        return await HttpCallOrchestrator<PaginationResult<GetTags.Response>>(
            () => webApiHttpClient.GetTagsAsync(query, cancellationToken), cancellationToken);
    }

    public async Task<WebClientResult> DeleteTagAsync(Guid tagId, CancellationToken cancellationToken)
    {
        return await HttpCallOrchestrator(() => webApiHttpClient.DeleteTagAsync(tagId, cancellationToken),
            cancellationToken);
    }

    public async Task<WebClientResult> UpdateTagAsync(
        Guid tagId,
        UpdateTagRequest request,
        CancellationToken cancellationToken)
    {
        return await HttpCallOrchestrator(() => webApiHttpClient.UpdateTagAsync(tagId, request, cancellationToken),
            cancellationToken);
    }
}