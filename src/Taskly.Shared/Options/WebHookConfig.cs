namespace Taskly.Shared.Options;

public sealed class WebHookConfig
{
    [Required(ErrorMessage = "SecretKey is required.")]
    public string SecretKey { get; init; } = null!;
}