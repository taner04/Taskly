namespace Api.Abstractions;

/// <summary>
/// Represents a base contract for requests associated with a specific user.
/// </summary>
public interface IUserRequestBase
{
    /// <summary>
    /// Gets or sets the identifier of the user on whose behalf the request is made.
    /// </summary>
    string UserId { get; set; }
}