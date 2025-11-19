namespace Api.Features.Shared.Domain;

public abstract class Entity<TId> : Auditable, IEntity<TId> where TId : struct
{
    public TId Id { get; init; }
}