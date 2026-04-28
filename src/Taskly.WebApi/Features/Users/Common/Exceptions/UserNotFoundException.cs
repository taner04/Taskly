namespace Taskly.WebApi.Features.Users.Common.Exceptions;

internal sealed class UserNotFoundException(string email)
    : TasklyException(
        "No user found by the provided email.",
        $"The user with the email '{email}' does not exist.",
        "User.NotFound",
        HttpStatusCode.NotFound);