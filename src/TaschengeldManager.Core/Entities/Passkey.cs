namespace TaschengeldManager.Core.Entities;

/// <summary>
/// Represents a WebAuthn passkey for a user.
/// </summary>
public class Passkey : BaseEntity<PasskeyId>
{
    /// <summary>
    /// User this passkey belongs to.
    /// </summary>
    public UserId UserId { get; set; }
    public User? User { get; set; }

    /// <summary>
    /// WebAuthn credential ID (base64 encoded).
    /// </summary>
    public string CredentialId { get; set; } = string.Empty;

    /// <summary>
    /// Public key for this passkey (base64 encoded).
    /// </summary>
    public string PublicKey { get; set; } = string.Empty;

    /// <summary>
    /// Sign counter to prevent replay attacks.
    /// </summary>
    public uint SignCount { get; set; }

    /// <summary>
    /// Name of the device/authenticator.
    /// </summary>
    public string DeviceName { get; set; } = string.Empty;

    /// <summary>
    /// AAGUID of the authenticator.
    /// </summary>
    public string? AaGuid { get; set; }

    /// <summary>
    /// Credential type (e.g., "public-key").
    /// </summary>
    public string CredentialType { get; set; } = "public-key";

    /// <summary>
    /// Transports supported (e.g., "internal", "usb", "nfc", "ble").
    /// </summary>
    public string? Transports { get; set; }

    /// <summary>
    /// When the passkey was last used.
    /// </summary>
    public DateTime? LastUsedAt { get; set; }
}
