using Taskly.WebApi.Common.Shared.Exceptions;

namespace Taskly.WebApi.Features.Users.Exceptions;

internal sealed class UserEmailNotFoundException(string email)
    : TasklyException(
        "No user found by the provided email.",
        $"The user with the email '{email}' does not exist.",
        "User.NotFound",
        HttpStatusCode.NotFound);