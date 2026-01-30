namespace TaschengeldManager.Core.Entities;

/// <summary>
/// Join table for the many-to-many relationship between families and relatives.
/// </summary>
public class FamilyRelative
{
    /// <summary>
    /// Family ID.
    /// </summary>
    public FamilyId FamilyId { get; set; }
    public Family? Family { get; set; }

    /// <summary>
    /// Relative user ID.
    /// </summary>
    public UserId UserId { get; set; }
    public User? User { get; set; }

    /// <summary>
    /// Date when the relative joined the family.
    /// </summary>
    public DateTime JoinedAt { get; set; }

    /// <summary>
    /// Relationship description (e.g., "Oma", "Onkel Peter").
    /// </summary>
    public string? RelationshipDescription { get; set; }
}
