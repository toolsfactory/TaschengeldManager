using TaschengeldManager.Core.Enums;

namespace TaschengeldManager.Core.Entities;

/// <summary>
/// Represents a biometric authentication token for mobile devices.
/// </summary>
public class BiometricToken : BaseEntity<BiometricTokenId>
{
    /// <summary>
    /// User this token belongs to.
    /// </summary>
    public UserId UserId { get; set; }
    public User? User { get; set; }

    /// <summary>
    /// Unique device identifier.
    /// </summary>
    public string DeviceId { get; set; } = string.Empty;

    /// <summary>
    /// Name of the device.
    /// </summary>
    public string DeviceName { get; set; } = string.Empty;

    /// <summary>
    /// Platform (iOS, Android).
    /// </summary>
    public string Platform { get; set; } = string.Empty;

    /// <summary>
    /// Type of biometry used.
    /// </summary>
    public BiometryType BiometryType { get; set; }

    /// <summary>
    /// Hash of the biometric token for verification.
    /// </summary>
    public string TokenHash { get; set; } = string.Empty;

    /// <summary>
    /// When the token expires (14 days after creation).
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// When the token was last used.
    /// </summary>
    public DateTime? LastUsedAt { get; set; }

    /// <summary>
    /// Whether the token is still valid.
    /// </summary>
    public bool IsValid { get; set; } = true;
}
