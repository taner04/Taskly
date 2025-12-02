namespace Api.Abstractions;

/// <summary>
///     Represents an entity that exposes an identifier of type <typeparamref name="TId" />.
/// </summary>
/// <typeparam name="TId">The value type used for the entity identifier. Must be a non-nullable value type.</typeparam>
public interface IEntity<TId> where TId : struct
{
    /// <summary>
    ///     The identifier of the entity.
    /// </summary>
    TId Id { get; init; }
}