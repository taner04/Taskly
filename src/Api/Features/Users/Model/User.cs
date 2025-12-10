using System.ComponentModel.DataAnnotations;
using Api.Features.Shared.Models;
using Api.Features.Users.Exceptions;

namespace Api.Features.Users.Model;

[ValueObject<Guid>]
public readonly partial struct UserId
{
    public static UserId EmptyId => From(Guid.Empty);
}

public sealed class User : Entity<UserId>
{
    private User(
        string email,
        string auth0Id)
    {
        Id = UserId.From(Guid.CreateVersion7());
        Email = email;
        Auth0Id = auth0Id;
    }

    public string Email { get; private set; }
    public string Auth0Id { get; private set; }

    public static User Create(
        string email,
        string auth0Id)
    {
        return !new EmailAddressAttribute().IsValid(email)
            ? throw new UserInvalidEmailException(email)
            : new User(email, auth0Id);
    }
}