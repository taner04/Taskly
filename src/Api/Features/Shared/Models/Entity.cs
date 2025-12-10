namespace Api.Features.Shared.Models;

public abstract class Entity<TId> : Auditable
    where TId : struct
{
    public TId Id { get; protected init; }
}