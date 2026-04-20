using Taskly.WebApi.Common.Shared.Pagination;
using Taskly.WebApi.Features.Users.Endpoints.Responses;

namespace Taskly.WebApi.Features.Users.Mapper;

internal sealed class UserMapper : IPaginationMapper<User, GetUserResponse>
{
    public List<GetUserResponse> Map(List<User> source)
    {
        return source.Select(u => new GetUserResponse(
            u.Id.Value,
            u.Email,
            u.Auth0Id
        )).ToList();
    }
}