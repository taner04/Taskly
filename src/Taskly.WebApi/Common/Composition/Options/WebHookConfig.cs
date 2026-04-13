using System.ComponentModel.DataAnnotations;

namespace Taskly.WebApi.Common.Composition.Options;

internal sealed class WebHookConfig
{
    [Required(ErrorMessage = "SecretKey is required.")]
    public string SecretKey { get; init; } = null!;
}