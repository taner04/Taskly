namespace Api.Abstractions;

/// <summary>
/// Represents an aggregate root in the domain.
/// An aggregate groups related domain objects under a single root and defines
/// a consistency and transactional boundary for that group.
/// </summary>
public interface IAggregate;