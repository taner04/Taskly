using Taskly.Desktop.Common.Shared.Models;

namespace Taskly.Desktop.Features.Todo.Models;

public sealed partial class AttachmentModel : Model
{
    [ObservableProperty] public partial string FileName { get; set; } = null!;
    [ObservableProperty] public partial long Size { get; set; }
    [ObservableProperty] public partial string ContentType { get; set; } = null!;
    [ObservableProperty] public partial string DownloadUrl { get; set; } = null!;
    [ObservableProperty] public partial bool IsDownloaded { get; set; }
}