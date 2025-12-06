namespace Api.Features.Shared.Models;

public abstract class Entity<TId> : Auditable
{
    public TId Id { get; init; }
}