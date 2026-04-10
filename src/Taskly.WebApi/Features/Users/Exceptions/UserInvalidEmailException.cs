using Taskly.WebApi.Common.Shared.Exceptions;

namespace Taskly.WebApi.Features.Users.Exceptions;

internal sealed class UserInvalidEmailException(string email)
    : TasklyException(
        "Invalid email format.",
        $"The provided email '{email}' is not in a valid format.",
        "User.InvalidEmail",
        HttpStatusCode.BadRequest);