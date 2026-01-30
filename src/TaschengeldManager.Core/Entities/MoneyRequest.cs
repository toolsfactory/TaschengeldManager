using TaschengeldManager.Core.Enums;

namespace TaschengeldManager.Core.Entities;

/// <summary>
/// Represents a money request from a child to parents.
/// </summary>
public class MoneyRequest : BaseEntity<MoneyRequestId>
{
    /// <summary>
    /// User ID of the child making the request.
    /// </summary>
    public UserId ChildUserId { get; set; }
    public User? ChildUser { get; set; }

    /// <summary>
    /// Requested amount.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Reason for the request.
    /// </summary>
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// Current status of the request.
    /// </summary>
    public RequestStatus Status { get; set; } = RequestStatus.Pending;

    /// <summary>
    /// Note from parent when responding (optional).
    /// </summary>
    public string? ResponseNote { get; set; }

    /// <summary>
    /// User ID of the parent who responded (optional).
    /// </summary>
    public UserId? RespondedByUserId { get; set; }
    public User? RespondedByUser { get; set; }

    /// <summary>
    /// When the request was responded to (optional).
    /// </summary>
    public DateTime? RespondedAt { get; set; }

    /// <summary>
    /// Transaction ID if request was approved and money was deposited.
    /// </summary>
    public TransactionId? ResultingTransactionId { get; set; }
    public Transaction? ResultingTransaction { get; set; }
}
