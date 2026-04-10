using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Taskly.WebApi.Common.Shared.Models;
using Taskly.WebApi.Features.Todos.Models;
using Taskly.WebApi.Features.Users.Exceptions;

namespace Taskly.WebApi.Features.Users.Models;

[ValueObject<Guid>]
public readonly partial struct UserId
{
    public static UserId EmptyId => UserId.From(Guid.Empty);
}

public sealed class User : Entity<UserId>
{
    [JsonConstructor]
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
    public List<Todo> Todos { get; private set; } = [];

    public static User Create(
        string email,
        string auth0Id) =>
        !new EmailAddressAttribute().IsValid(email)
            ? throw new UserInvalidEmailException(email)
            : new User(email, auth0Id);
}