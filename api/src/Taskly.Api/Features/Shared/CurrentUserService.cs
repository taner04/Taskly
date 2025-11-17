namespace Taskly.Api.Features.Shared;

public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor)
{
    public string? GetCurrentUserId()
    {
        var userId = httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;

        return string.IsNullOrEmpty(userId) ? null : userId;
    }
}