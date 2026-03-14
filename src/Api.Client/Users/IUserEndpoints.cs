using Api.Features.Shared;
using Api.Features.Users.Model;
using Refit;

namespace Api.Client.Users;

public interface IUserEndpoints
{
    [Get(ApiRoutes.Users.GetUsers)]
    Task<HttpResponseMessage> GetUsersAsync(
        CancellationToken cancellationToken = default);

    [Delete(ApiRoutes.Users.RemoveUser)]
    Task<HttpResponseMessage> RemoveUserAsync(
        UserId userId,
        CancellationToken cancellationToken = default);

    [Get(ApiRoutes.Users.GetByEmail)]
    Task<HttpResponseMessage> GetUserByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);
}