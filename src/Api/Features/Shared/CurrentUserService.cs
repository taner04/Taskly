namespace Api.Features.Shared;

public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor)
{
    public string GetCurrentUserId()
    {
        var userId = httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;

        return string.IsNullOrEmpty(userId)
            ? throw new UnauthorizedAccessException("The user is not authenticated.")
            : userId;
    }
}