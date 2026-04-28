using Taskly.Shared.WebApi.Responses.Users;

namespace Taskly.WebApi.Client.Common.Services;

public sealed class UserService(IRefitWebApiClient webApiHttpClient) : IUserService
{
    public async Task<WebClientResult> DeleteUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await HttpOrchestrator.OrchestrateAsync(() => webApiHttpClient.DeleteUserAsync(userId, cancellationToken),
            cancellationToken);
    }

    public async Task<WebClientResult<GetUserResponse>> GetUserByEmailAsync(
        string email,
        CancellationToken cancellationToken)
    {
        return await HttpOrchestrator.OrchestrateAsync<GetUserResponse>(
            () => webApiHttpClient.GetUserByEmailAsync(email, cancellationToken), cancellationToken);
    }

    public async Task<WebClientResult<PaginationResult<GetUserResponse>>> GetUserAsync(
        PaginationQuery query,
        CancellationToken cancellationToken)
    {
        return await HttpOrchestrator.OrchestrateAsync<PaginationResult<GetUserResponse>>(
            () => webApiHttpClient.GetUsersAsync(query, cancellationToken), cancellationToken);
    }
}