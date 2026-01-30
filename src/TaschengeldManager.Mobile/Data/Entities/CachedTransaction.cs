using SQLite;
using TaschengeldManager.Core.Enums;

namespace TaschengeldManager.Mobile.Data.Entities;

/// <summary>
/// Cached transaction entity for offline storage
/// </summary>
[Table("Transactions")]
public class CachedTransaction
{
    [PrimaryKey]
    public Guid Id { get; set; }

    /// <summary>
    /// Local reference to account (not in DTO but needed for local queries)
    /// </summary>
    [Indexed]
    public Guid AccountId { get; set; }

    public TransactionType Type { get; set; }

    public decimal Amount { get; set; }

    public string? Description { get; set; }

    public string? Category { get; set; }

    public string CreatedByName { get; set; } = string.Empty;

    public decimal BalanceAfter { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime CachedAt { get; set; }
}
