namespace Api.Abstractions;

/// <summary>
///     Represents an auditable entity with creation and update metadata.
/// </summary>
public interface IAuditable
{
    /// <summary>
    ///     Gets the date and time when the entity was created.
    /// </summary>
    DateTimeOffset CreatedAt { get; }

    /// <summary>
    ///     Gets the date and time when the entity was last updated.
    /// </summary>
    DateTimeOffset? UpdatedAt { get; }

    /// <summary>
    ///     Gets the identifier of the user who created the entity.
    /// </summary>
    string CreatedBy { get; }

    /// <summary>
    ///     Gets the identifier of the user who last updated the entity.
    /// </summary>
    string? UpdatedBy { get; }

    /// <summary>
    ///     Sets the creation metadata for the entity.
    /// </summary>
    /// <param name="createdBy">The identifier of the user who created the entity.</param>
    void SetCreated(string createdBy = null!);

    /// <summary>
    ///     Sets the update metadata for the entity.
    /// </summary>
    /// <param name="updatedBy">The identifier of the user who updated the entity.</param>
    void SetUpdated(string updatedBy = null!);
}