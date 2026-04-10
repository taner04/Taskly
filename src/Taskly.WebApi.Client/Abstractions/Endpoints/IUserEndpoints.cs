using Refit;
using Taskly.WebApi.Common.Shared;
using Taskly.WebApi.Features.Users.Endpoints;
using UserId = Taskly.WebApi.Features.Users.Models.UserId;

namespace Taskly.WebApi.Client.Abstractions.Endpoints;

public interface IUserEndpoints
{
    [Get(ApiRoutes.Users.GetUsers)]
    Task<HttpResponseMessage> GetUsersAsync(
        GetUsers.Query query,
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