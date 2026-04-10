using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Taskly.WebApi.Common.Shared.Models;

namespace Taskly.WebApi.Features.Users.Models;

[ValueObject<Guid>]
public readonly partial struct UserId
{
    public static UserId EmptyId => From(Guid.Empty);
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

    [EmailAddress] public string Email { get; private set; }

    public string Auth0Id { get; private set; }
    public ICollection<Todo> Todos { get; private set; } = [];

    public static User Create(string email, string auth0Id) => new(email, auth0Id);
}