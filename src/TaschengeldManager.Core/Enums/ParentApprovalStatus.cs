namespace TaschengeldManager.Core.Enums;

/// <summary>
/// Status of a parent approval request (MFA alternative for children).
/// </summary>
public enum ParentApprovalStatus
{
    /// <summary>
    /// Waiting for parent response.
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Parent approved the login.
    /// </summary>
    Approved = 1,

    /// <summary>
    /// Parent rejected the login.
    /// </summary>
    Rejected = 2,

    /// <summary>
    /// Request expired (after 5 minutes).
    /// </summary>
    Expired = 3
}
