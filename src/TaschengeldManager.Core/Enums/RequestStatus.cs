namespace TaschengeldManager.Core.Enums;

/// <summary>
/// Status of a money request from child to parent.
/// </summary>
public enum RequestStatus
{
    /// <summary>
    /// Request is pending approval.
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Request has been approved by parent.
    /// </summary>
    Approved = 1,

    /// <summary>
    /// Request has been rejected by parent.
    /// </summary>
    Rejected = 2,

    /// <summary>
    /// Request has been withdrawn by child.
    /// </summary>
    Withdrawn = 3
}
