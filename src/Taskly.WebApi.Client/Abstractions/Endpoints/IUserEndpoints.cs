using Taskly.WebApi.Features.Shared;
using Taskly.WebApi.Features.Users.Model;
using Refit;

namespace Taskly.WebApi.Client.Abstractions.Endpoints;

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
