using Taskly.Shared.Pagination;
using Taskly.WebApi.Features.Users.Endpoints.Responses;

namespace Taskly.WebApi.Client.Features.Users.Services;

public sealed class UserService(IRefitWebApiClient webApiHttpClient) : EndpointService
{
    public async Task<WebClientResult> DeleteUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await HttpCallOrchestrator(() => webApiHttpClient.DeleteUserAsync(userId, cancellationToken),
            cancellationToken);
    }

    public async Task<WebClientResult<GetUserResponse>> GetUserByEmailAsync(
        string email,
        CancellationToken cancellationToken)
    {
        return await HttpCallOrchestrator<GetUserResponse>(
            () => webApiHttpClient.GetUserByEmailAsync(email, cancellationToken), cancellationToken);
    }

    public async Task<WebClientResult<PaginationResult<GetUserResponse>>> GetUserAsync(
        PaginationQuery query,
        CancellationToken cancellationToken)
    {
        return await HttpCallOrchestrator<PaginationResult<GetUserResponse>>(
            () => webApiHttpClient.GetUsersAsync(query, cancellationToken), cancellationToken);
    }
}