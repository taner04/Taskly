namespace Taskly.WebApi.Features.Users.Exceptions;

internal sealed class UserEmailNotFoundException(string email)
    : ModelBaseException(
        "No user found by the provided email.",
        $"The user with the email '{email}' does not exist.",
        "User.NotFound",
        HttpStatusCode.NotFound);
