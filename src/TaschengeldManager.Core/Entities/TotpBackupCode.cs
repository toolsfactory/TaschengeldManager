namespace TaschengeldManager.Core.Entities;

/// <summary>
/// Represents a TOTP backup code for account recovery.
/// </summary>
public class TotpBackupCode : BaseEntity<TotpBackupCodeId>
{
    /// <summary>
    /// User this backup code belongs to.
    /// </summary>
    public UserId UserId { get; set; }
    public User? User { get; set; }

    /// <summary>
    /// Hash of the backup code.
    /// </summary>
    public string CodeHash { get; set; } = string.Empty;

    /// <summary>
    /// Whether the code has been used.
    /// </summary>
    public bool IsUsed { get; set; }

    /// <summary>
    /// When the code was used.
    /// </summary>
    public DateTime? UsedAt { get; set; }
}
