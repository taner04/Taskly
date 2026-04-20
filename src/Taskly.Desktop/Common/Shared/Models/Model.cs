namespace Taskly.Desktop.Common.Shared.Models;

public abstract class Model : ObservableObject
{
    public Guid Id { get; init; }
}