namespace TaschengeldManager.Core.Entities;

/// <summary>
/// Represents a family group containing parents, children, and relatives.
/// </summary>
public class Family : BaseEntity<FamilyId>
{
    /// <summary>
    /// Name of the family (e.g., "Familie Müller").
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Unique code for child login (6 characters).
    /// </summary>
    public string FamilyCode { get; set; } = string.Empty;

    /// <summary>
    /// User who created this family.
    /// </summary>
    public UserId CreatedByUserId { get; set; }
    public User? CreatedByUser { get; set; }

    // Navigation properties

    /// <summary>
    /// Parents in this family (many-to-many).
    /// </summary>
    public ICollection<FamilyParent> Parents { get; set; } = new List<FamilyParent>();

    /// <summary>
    /// Relatives in this family (many-to-many).
    /// </summary>
    public ICollection<FamilyRelative> Relatives { get; set; } = new List<FamilyRelative>();

    /// <summary>
    /// Children in this family (one-to-many).
    /// </summary>
    public ICollection<User> Children { get; set; } = new List<User>();

    /// <summary>
    /// Pending invitations for this family.
    /// </summary>
    public ICollection<FamilyInvitation> Invitations { get; set; } = new List<FamilyInvitation>();
}
