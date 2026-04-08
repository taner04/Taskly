using System.ComponentModel.DataAnnotations;

namespace Taskly.Desktop.Common.Composition.Options;

internal sealed class Auth0Config
{
    [Required(ErrorMessage = "Auth0 domain is required.")]
    public string Domain { get; init; } = null!;

    [Required(ErrorMessage = "Auth0 client id is required.")]
    public string ClientId { get; init; } = null!;
}
