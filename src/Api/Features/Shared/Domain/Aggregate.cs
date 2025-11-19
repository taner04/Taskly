namespace Api.Features.Shared.Domain;

public abstract class Aggregate<TId> : Entity<TId>, IAggregate where TId : struct;