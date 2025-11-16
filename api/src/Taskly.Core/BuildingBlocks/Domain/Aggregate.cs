namespace Taskly.Core.BuildingBlocks.Domain;

public abstract class Aggregate<TId> : Entity<TId>, IAggregate where TId : struct;