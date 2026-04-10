using System.ComponentModel.DataAnnotations;

namespace Taskly.WebApi.Common.Composition.Options;

public sealed class EmailConfig
{
    [Required(ErrorMessage = "SMTP server is required.")]
    [RegularExpression(@"^[a-zA-Z0-9.-]+$",
        ErrorMessage = "SMTP server must be a valid hostname without protocol or path.")]
    public string Host { get; set; } = null!;

    [Required(ErrorMessage = "Port is required.")]
    [Range(1, 65535, ErrorMessage = "Port must be between 1 and 65535.")]
    public int Port { get; set; }
}