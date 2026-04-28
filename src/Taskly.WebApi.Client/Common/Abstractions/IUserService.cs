using Taskly.Shared.WebApi.Responses.Users;

namespace Taskly.WebApi.Client.Common.Abstractions;

public interface IUserService
{
    Task<WebClientResult> DeleteUserAsync(Guid userId, CancellationToken cancellationToken);

    Task<WebClientResult<GetUserResponse>> GetUserByEmailAsync(string email, CancellationToken cancellationToken);

    Task<WebClientResult<PaginationResult<GetUserResponse>>> GetUserAsync(
        PaginationQuery query,
        CancellationToken cancellationToken);
}