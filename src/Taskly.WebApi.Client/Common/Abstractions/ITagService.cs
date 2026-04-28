using Taskly.Shared.WebApi.Responses.Tags;
using Taskly.WebApi.Client.Common.Dtos.Tags;

namespace Taskly.WebApi.Client.Common.Abstractions;

public interface ITagService
{
    Task<WebClientResult<GetTagResponse>> CreateTagAsync(
        CreateTagRequest request,
        CancellationToken cancellationToken);

    Task<WebClientResult<PaginationResult<GetTagResponse>>> GetTagsAsync(
        PaginationQuery query,
        CancellationToken cancellationToken = default);

    Task<WebClientResult> DeleteTagAsync(Guid tagId, CancellationToken cancellationToken = default);

    Task<WebClientResult<GetTagResponse>> UpdateTagAsync(
        Guid tagId,
        UpdateTagRequest request,
        CancellationToken cancellationToken);
}