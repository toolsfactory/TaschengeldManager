namespace TaschengeldManager.Core.Entities;

/// <summary>
/// Base class for all entities with common properties.
/// </summary>
/// <typeparam name="TId">The strongly typed ID type for this entity.</typeparam>
public abstract class BaseEntity<TId> : IHasTimestamps where TId : struct
{
    /// <summary>
    /// Unique identifier.
    /// </summary>
    public TId Id { get; set; }

    /// <summary>
    /// Date and time when the entity was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date and time when the entity was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
