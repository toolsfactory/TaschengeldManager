using TaschengeldManager.Core.Enums;

namespace TaschengeldManager.Core.Entities;

/// <summary>
/// Represents a transaction on an account.
/// </summary>
public class Transaction : BaseEntity<TransactionId>
{
    /// <summary>
    /// Account this transaction belongs to.
    /// </summary>
    public AccountId AccountId { get; set; }
    public Account? Account { get; set; }

    /// <summary>
    /// Amount of the transaction (positive for deposits, negative for withdrawals).
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Type of transaction.
    /// </summary>
    public TransactionType Type { get; set; }

    /// <summary>
    /// Description of the transaction.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Category of the transaction (for expenses).
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// User who created this transaction (null for system-generated transactions like interest).
    /// </summary>
    public UserId? CreatedByUserId { get; set; }
    public User? CreatedByUser { get; set; }

    /// <summary>
    /// Balance after this transaction.
    /// </summary>
    public decimal BalanceAfter { get; set; }
}
