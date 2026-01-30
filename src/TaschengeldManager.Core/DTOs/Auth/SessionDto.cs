namespace TaschengeldManager.Core.DTOs.Auth;

/// <summary>
/// Session information for display.
/// </summary>
public record SessionDto
{
    /// <summary>
    /// Session ID.
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// Device information.
    /// </summary>
    public string? DeviceInfo { get; init; }

    /// <summary>
    /// IP address.
    /// </summary>
    public string? IpAddress { get; init; }

    /// <summary>
    /// When the session was created.
    /// </summary>
    public required DateTime CreatedAt { get; init; }

    /// <summary>
    /// Last activity timestamp.
    /// </summary>
    public required DateTime LastActivityAt { get; init; }

    /// <summary>
    /// Whether this is the current session.
    /// </summary>
    public bool IsCurrent { get; init; }

    /// <summary>
    /// Whether this is a trusted device.
    /// </summary>
    public bool IsTrustedDevice { get; init; }
}
