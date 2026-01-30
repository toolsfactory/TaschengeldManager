namespace TaschengeldManager.Core.Entities;

/// <summary>
/// Represents an active user session.
/// </summary>
public class Session : BaseEntity<SessionId>
{
    /// <summary>
    /// User this session belongs to.
    /// </summary>
    public UserId UserId { get; set; }
    public User? User { get; set; }

    /// <summary>
    /// Hash of the refresh token.
    /// </summary>
    public string RefreshTokenHash { get; set; } = string.Empty;

    /// <summary>
    /// Device information (user agent, device name).
    /// </summary>
    public string? DeviceInfo { get; set; }

    /// <summary>
    /// IP address of the session.
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// Last activity timestamp.
    /// </summary>
    public DateTime LastActivityAt { get; set; }

    /// <summary>
    /// When the session expires.
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Whether the session has been revoked.
    /// </summary>
    public bool IsRevoked { get; set; }

    /// <summary>
    /// When the session was revoked.
    /// </summary>
    public DateTime? RevokedAt { get; set; }

    /// <summary>
    /// Whether this is a trusted device (biometric enabled).
    /// </summary>
    public bool IsTrustedDevice { get; set; }
}
