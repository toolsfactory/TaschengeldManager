namespace TaschengeldManager.Core.Enums;

/// <summary>
/// Status of a family invitation.
/// </summary>
public enum InvitationStatus
{
    /// <summary>
    /// Invitation is pending and waiting for response.
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Invitation was accepted.
    /// </summary>
    Accepted = 1,

    /// <summary>
    /// Invitation was rejected by the invitee.
    /// </summary>
    Rejected = 2,

    /// <summary>
    /// Invitation expired (after 7 days).
    /// </summary>
    Expired = 3,

    /// <summary>
    /// Invitation was withdrawn by the inviter.
    /// </summary>
    Withdrawn = 4
}
