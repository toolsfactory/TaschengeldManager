using TaschengeldManager.Core.Enums;

namespace TaschengeldManager.Core.Entities;

/// <summary>
/// Represents an invitation to join a family as parent or relative.
/// </summary>
public class FamilyInvitation : BaseEntity<FamilyInvitationId>
{
    /// <summary>
    /// Family the invitation is for.
    /// </summary>
    public FamilyId FamilyId { get; set; }
    public Family? Family { get; set; }

    /// <summary>
    /// Email address of the invited person.
    /// </summary>
    public string InvitedEmail { get; set; } = string.Empty;

    /// <summary>
    /// Normalized email for lookup.
    /// </summary>
    public string NormalizedInvitedEmail { get; set; } = string.Empty;

    /// <summary>
    /// User who sent the invitation.
    /// </summary>
    public UserId InvitedByUserId { get; set; }
    public User? InvitedByUser { get; set; }

    /// <summary>
    /// Role the invited person will have (Parent or Relative).
    /// </summary>
    public UserRole InvitedRole { get; set; }

    /// <summary>
    /// Current status of the invitation.
    /// </summary>
    public InvitationStatus Status { get; set; }

    /// <summary>
    /// Secure token for the invitation link.
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Hash of the token for secure lookup.
    /// </summary>
    public string TokenHash { get; set; } = string.Empty;

    /// <summary>
    /// When the invitation expires (7 days after creation).
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// When the invitation was responded to.
    /// </summary>
    public DateTime? RespondedAt { get; set; }

    /// <summary>
    /// Optional relationship description for relatives.
    /// </summary>
    public string? RelationshipDescription { get; set; }
}
