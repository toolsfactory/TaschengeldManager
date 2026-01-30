namespace TaschengeldManager.Core.Entities;

/// <summary>
/// Interface for entities with timestamp properties.
/// </summary>
public interface IHasTimestamps
{
    DateTime CreatedAt { get; set; }
    DateTime? UpdatedAt { get; set; }
}
