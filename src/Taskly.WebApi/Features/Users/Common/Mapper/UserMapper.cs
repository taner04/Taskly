using Taskly.Shared.WebApi.Responses.Users;
using Taskly.WebApi.Common.Shared.Pagination;
using Taskly.WebApi.Features.Users.Common.Models;

namespace Taskly.WebApi.Features.Users.Common.Mapper;

internal sealed class UserMapper : IPaginationMapper<User, GetUserResponse>
{
    public IEnumerable<GetUserResponse> Map(IEnumerable<User> source)
    {
        return source.Select(u => new GetUserResponse(
            u.Id.Value,
            u.Email,
            u.Auth0Id
        )).ToList();
    }
}