namespace Taskly.Core.BuildingBlocks.Domain;

public abstract class Auditable : IAuditable
{
    public DateTimeOffset CreatedAt { get; private set; }
    public string CreatedBy { get; private set; } = null!;

    public DateTimeOffset? UpdatedAt { get; private set; }
    public string? UpdatedBy { get; private set; }


    public void SetCreated(string createdBy = null!)
    {
        CreatedAt = DateTimeOffset.UtcNow;
        CreatedBy = createdBy ?? "System";
    }

    public void SetUpdated(string updatedBy = null!)
    {
        UpdatedAt = DateTimeOffset.UtcNow;
        UpdatedBy = updatedBy ?? "System";
    }
}