using System.Collections.ObjectModel;
using Taskly.Desktop.Common.Shared.Models;
using Taskly.Desktop.Features.Tag.Models;

namespace Taskly.Desktop.Features.Todo.Models;

public sealed partial class TodoModel : Model
{
    [ObservableProperty] public partial string Title { get; set; } = null!;
    [ObservableProperty] public partial string? Description { get; set; }
    [ObservableProperty] public partial int Priority { get; set; }
    [ObservableProperty] public partial bool IsCompleted { get; set; }
    public DateTimeOffset CreatedAt { get; init; }
    [ObservableProperty] public partial ObservableCollection<TagModel> Tags { get; set; } = [];
    [ObservableProperty] public partial ObservableCollection<AttachmentModel> Attachments { get; set; } = [];
}