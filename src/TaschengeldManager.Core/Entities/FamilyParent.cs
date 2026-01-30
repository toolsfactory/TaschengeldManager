namespace TaschengeldManager.Core.Entities;

/// <summary>
/// Join table for the many-to-many relationship between families and parents.
/// </summary>
public class FamilyParent
{
    /// <summary>
    /// Family ID.
    /// </summary>
    public FamilyId FamilyId { get; set; }
    public Family? Family { get; set; }

    /// <summary>
    /// Parent user ID.
    /// </summary>
    public UserId UserId { get; set; }
    public User? User { get; set; }

    /// <summary>
    /// Date when the parent joined the family.
    /// </summary>
    public DateTime JoinedAt { get; set; }

    /// <summary>
    /// Whether this parent is the primary (creator) of the family.
    /// </summary>
    public bool IsPrimary { get; set; }
}
