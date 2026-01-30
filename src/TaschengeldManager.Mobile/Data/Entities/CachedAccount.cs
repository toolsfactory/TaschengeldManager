using SQLite;

namespace TaschengeldManager.Mobile.Data.Entities;

/// <summary>
/// Cached account entity for offline storage
/// </summary>
[Table("Accounts")]
public class CachedAccount
{
    [PrimaryKey]
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string OwnerName { get; set; } = string.Empty;

    public decimal Balance { get; set; }

    public bool InterestEnabled { get; set; }

    public decimal? InterestRate { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime CachedAt { get; set; }

    public DateTime? LastSyncedAt { get; set; }
}
