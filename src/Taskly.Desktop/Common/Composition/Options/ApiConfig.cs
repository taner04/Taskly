using System.ComponentModel.DataAnnotations;

namespace Taskly.Desktop.Common.Composition.Options;

public sealed class ApiConfig
{
    [Required(ErrorMessage = "API base URL is required.")]
    [Url(ErrorMessage = "API base URL must be a valid URL.")]
    public string BaseAddress { get; init; } = null!;

    [Required(ErrorMessage = "API timeout is required.")]
    public int TimeoutInSeconds { get; init; }
}