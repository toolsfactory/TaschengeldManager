using TaschengeldManager.Core.Enums;

namespace TaschengeldManager.Core.Entities;

/// <summary>
/// Represents a parent approval request for child login (MFA alternative).
/// </summary>
public class ParentApprovalRequest : BaseEntity<ParentApprovalRequestId>
{
    /// <summary>
    /// Child user requesting approval.
    /// </summary>
    public UserId ChildUserId { get; set; }
    public User? ChildUser { get; set; }

    /// <summary>
    /// Family for context.
    /// </summary>
    public FamilyId FamilyId { get; set; }
    public Family? Family { get; set; }

    /// <summary>
    /// Device info for the request.
    /// </summary>
    public string? DeviceInfo { get; set; }

    /// <summary>
    /// Approximate location of the request.
    /// </summary>
    public string? ApproximateLocation { get; set; }

    /// <summary>
    /// Current status of the request.
    /// </summary>
    public ParentApprovalStatus Status { get; set; }

    /// <summary>
    /// Parent who responded to the request.
    /// </summary>
    public UserId? RespondedByUserId { get; set; }
    public User? RespondedByUser { get; set; }

    /// <summary>
    /// When the request expires (5 minutes).
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// When the request was responded to.
    /// </summary>
    public DateTime? RespondedAt { get; set; }
}
