// ReSharper disable CheckNamespace

using Api.Features.Shared;
using Api.Features.Users.Model;
using Refit;

namespace Api.Client;

public partial interface IApiClient
{
    [Get(Routes.Users.GetUsers)]
    Task<HttpResponseMessage> GetUsersAsync(
        CancellationToken cancellationToken = default);

    [Delete(Routes.Users.RemoveUser)]
    Task<HttpResponseMessage> RemoveUserAsync(
        UserId userId,
        CancellationToken cancellationToken = default);

    [Get(Routes.Users.GetByEmail)]
    Task<HttpResponseMessage> GetUserByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);
}