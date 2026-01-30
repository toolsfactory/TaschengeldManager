namespace TaschengeldManager.Core.Entities;

/// <summary>
/// Represents a login attempt for audit purposes.
/// </summary>
public class LoginAttempt : BaseEntity<LoginAttemptId>
{
    /// <summary>
    /// User ID if known (nullable for failed attempts with unknown email).
    /// </summary>
    public UserId? UserId { get; set; }
    public User? User { get; set; }

    /// <summary>
    /// Email or nickname used for login attempt.
    /// </summary>
    public string Identifier { get; set; } = string.Empty;

    /// <summary>
    /// Whether the login was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Reason for failure if unsuccessful.
    /// </summary>
    public string? FailureReason { get; set; }

    /// <summary>
    /// IP address of the attempt.
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// User agent string.
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// MFA method used (if any).
    /// </summary>
    public string? MfaMethod { get; set; }

    /// <summary>
    /// Approximate location based on IP.
    /// </summary>
    public string? Location { get; set; }
}
