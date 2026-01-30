using SQLite;

namespace TaschengeldManager.Mobile.Data.Entities;

/// <summary>
/// Cached child entity for offline storage
/// </summary>
[Table("Children")]
public class CachedChild
{
    [PrimaryKey]
    public Guid Id { get; set; }

    /// <summary>
    /// Local reference to family (not in DTO but needed for local queries)
    /// </summary>
    [Indexed]
    public Guid FamilyId { get; set; }

    public Guid? AccountId { get; set; }

    public string Nickname { get; set; } = string.Empty;

    public decimal? Balance { get; set; }

    public bool IsLocked { get; set; }

    public bool MfaEnabled { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime CachedAt { get; set; }
}
