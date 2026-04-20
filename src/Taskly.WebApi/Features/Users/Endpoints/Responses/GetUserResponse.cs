namespace Taskly.WebApi.Features.Users.Endpoints.Responses;

public sealed record GetUserResponse(Guid UserId, string Email, string Auth0Id);