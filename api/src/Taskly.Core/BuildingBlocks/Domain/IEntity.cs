namespace Taskly.Core.BuildingBlocks.Domain;

public interface IEntity<TId> where TId : struct
{
    TId Id { get; init; }
}