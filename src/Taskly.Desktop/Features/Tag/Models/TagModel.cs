using Taskly.Desktop.Common.Shared.Models;

namespace Taskly.Desktop.Features.Tag.Models;

public sealed partial class TagModel : Model
{
    [ObservableProperty] public partial string Name { get; set; } = null!;
}