using SQLite;

namespace TaschengeldManager.Mobile.Data.Entities;

/// <summary>
/// Cached family entity for offline storage
/// </summary>
[Table("Families")]
public class CachedFamily
{
    [PrimaryKey]
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string FamilyCode { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime CachedAt { get; set; }

    public DateTime? LastSyncedAt { get; set; }
}
