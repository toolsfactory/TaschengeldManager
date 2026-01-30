using TaschengeldManager.Core.Enums;

namespace TaschengeldManager.Core.DTOs.Account;

/// <summary>
/// Transaction information for display.
/// </summary>
public record TransactionDto
{
    /// <summary>
    /// Transaction ID.
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// Amount (positive for deposits, negative for withdrawals).
    /// </summary>
    public required decimal Amount { get; init; }

    /// <summary>
    /// Transaction type.
    /// </summary>
    public required TransactionType Type { get; init; }

    /// <summary>
    /// Description.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Category.
    /// </summary>
    public string? Category { get; init; }

    /// <summary>
    /// Who created the transaction.
    /// </summary>
    public required string CreatedByName { get; init; }

    /// <summary>
    /// Balance after this transaction.
    /// </summary>
    public required decimal BalanceAfter { get; init; }

    /// <summary>
    /// When the transaction was created.
    /// </summary>
    public required DateTime CreatedAt { get; init; }
}
