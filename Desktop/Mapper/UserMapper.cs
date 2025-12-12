using Desktop.Shared;
using System.Security.Claims;

namespace Desktop.Mapper;

public static class UserMapper
{
    public static UserContext FromClaims(ClaimsPrincipal user)
    {
        return new UserContext
        {
            Id = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "",
            Email = user.FindFirst(ClaimTypes.Email)?.Value ?? "",
            Name = user.FindFirst("name")?.Value ?? "",
            PictureUrl = user.FindFirst("picture")?.Value,
            Roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList()
        };
    }
}
