namespace TaschengeldManager.Core.Configuration;

/// <summary>
/// Authentication and security-related settings.
/// </summary>
public class AuthSettings
{
    /// <summary>
    /// Configuration section name.
    /// </summary>
    public const string SectionName = "Auth";

    /// <summary>
    /// JWT access token expiration in minutes.
    /// </summary>
    public int AccessTokenExpirationMinutes { get; set; } = 15;

    /// <summary>
    /// Refresh token expiration in days.
    /// </summary>
    public int RefreshTokenExpirationDays { get; set; } = 7;

    /// <summary>
    /// Biometric token expiration in days.
    /// </summary>
    public int BiometricTokenExpirationDays { get; set; } = 14;

    /// <summary>
    /// Family invitation expiration in days.
    /// </summary>
    public int InvitationExpirationDays { get; set; } = 7;

    /// <summary>
    /// Maximum failed login attempts before lockout.
    /// </summary>
    public int MaxFailedLoginAttempts { get; set; } = 5;

    /// <summary>
    /// Lockout durations in minutes for progressive lockout.
    /// </summary>
    public int[] LockoutDurationsMinutes { get; set; } = [5, 15, 60, 1440];

    /// <summary>
    /// Number of backup codes to generate.
    /// </summary>
    public int BackupCodeCount { get; set; } = 10;
}
