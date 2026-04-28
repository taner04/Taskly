using Taskly.Shared.WebApi.Responses.Tags;
using Taskly.WebApi.Client.Common.Dtos.Tags;

namespace Taskly.WebApi.Client.Common.Services;

public sealed class TagService(IRefitWebApiClient webApiHttpClient) : ITagService
{
    public async Task<WebClientResult<GetTagResponse>> CreateTagAsync(
        CreateTagRequest request,
        CancellationToken cancellationToken)
    {
        return await HttpOrchestrator.OrchestrateAsync<GetTagResponse>(
            () => webApiHttpClient.CreateTagAsync(request, cancellationToken),
            cancellationToken);
    }

    public async Task<WebClientResult<PaginationResult<GetTagResponse>>> GetTagsAsync(
        PaginationQuery query,
        CancellationToken cancellationToken = default)
    {
        return await HttpOrchestrator.OrchestrateAsync<PaginationResult<GetTagResponse>>(
            () => webApiHttpClient.GetTagsAsync(query, cancellationToken), cancellationToken);
    }

    public async Task<WebClientResult> DeleteTagAsync(Guid tagId, CancellationToken cancellationToken = default)
    {
        return await HttpOrchestrator.OrchestrateAsync(() => webApiHttpClient.DeleteTagAsync(tagId, cancellationToken),
            cancellationToken);
    }

    public async Task<WebClientResult<GetTagResponse>> UpdateTagAsync(
        Guid tagId,
        UpdateTagRequest request,
        CancellationToken cancellationToken)
    {
        return await HttpOrchestrator.OrchestrateAsync<GetTagResponse>(
            () => webApiHttpClient.UpdateTagAsync(tagId, request, cancellationToken),
            cancellationToken);
    }
}