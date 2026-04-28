namespace Taskly.Shared.WebApi.Responses.Users;

public sealed record GetUserResponse(Guid UserId, string Email, string Auth0Id);