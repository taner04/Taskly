namespace Api.Features.Users.Exceptions;

internal sealed class UserInvalidEmailException(string email)
    : ModelBaseException(
        "Invalid email format.",
        $"The provided email '{email}' is not in a valid format.",
        "User.InvalidEmail",
        HttpStatusCode.BadRequest);