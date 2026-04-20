using Taskly.Shared.Pagination;

namespace Taskly.WebApi.Client.Common.Abstractions.Refit.Endpoints;

public interface IUserEndpoints
{
    [Get(ApiRoutes.Users.GetUsers)]
    Task<HttpResponseMessage> GetUsersAsync(
        PaginationQuery query,
        CancellationToken cancellationToken = default);

    [Delete(ApiRoutes.Users.RemoveUser)]
    Task<HttpResponseMessage> DeleteUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    [Get(ApiRoutes.Users.GetByEmail)]
    Task<HttpResponseMessage> GetUserByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);
}