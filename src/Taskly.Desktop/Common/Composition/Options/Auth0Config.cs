using System.ComponentModel.DataAnnotations;

namespace Taskly.Desktop.Common.Composition.Options;

public sealed class Auth0Config
{
    [Required(ErrorMessage = "Auth0 domain is required.")]
    public string Domain { get; init; } = null!;

    [Required(ErrorMessage = "Auth0 client id is required.")]
    public string ClientId { get; init; } = null!;

    [Required(ErrorMessage = "Auth0 scope is required.")]
    public string Scope { get; init; } = null!;

    [Required(ErrorMessage = "Auth0 audience is required.")]
    public string Audience { get; init; } = null!;

    [Required(ErrorMessage = "Auth0 connection name is required.")]
    public string ConnectionName { get; init; } = null!;
}