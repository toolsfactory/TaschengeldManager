using TaschengeldManager.Core.Enums;

namespace TaschengeldManager.Core.Entities;

/// <summary>
/// Represents a user in the system (Parent, Child, or Relative).
/// </summary>
public class User : BaseEntity<UserId>
{
    /// <summary>
    /// Email address (nullable for children without own login).
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Normalized email for case-insensitive lookup.
    /// </summary>
    public string? NormalizedEmail { get; set; }

    /// <summary>
    /// Hashed password (Argon2id).
    /// </summary>
    public string? PasswordHash { get; set; }

    /// <summary>
    /// Nickname for display and child login.
    /// </summary>
    public string Nickname { get; set; } = string.Empty;

    /// <summary>
    /// PIN for child login (hashed).
    /// </summary>
    public string? PinHash { get; set; }

    /// <summary>
    /// User's role in the system.
    /// </summary>
    public UserRole Role { get; set; }

    /// <summary>
    /// Whether MFA is enabled (always true after initial setup).
    /// </summary>
    public bool MfaEnabled { get; set; }

    /// <summary>
    /// Encrypted TOTP secret for authenticator apps.
    /// </summary>
    public string? TotpSecret { get; set; }

    /// <summary>
    /// Whether the account is locked.
    /// </summary>
    public bool IsLocked { get; set; }

    /// <summary>
    /// Reason for account lock.
    /// </summary>
    public string? LockReason { get; set; }

    /// <summary>
    /// Date when account was locked.
    /// </summary>
    public DateTime? LockedAt { get; set; }

    /// <summary>
    /// User who locked this account (for children locked by parents).
    /// </summary>
    public UserId? LockedByUserId { get; set; }

    /// <summary>
    /// Number of consecutive failed login attempts.
    /// </summary>
    public int FailedLoginAttempts { get; set; }

    /// <summary>
    /// End time of temporary lockout due to failed attempts.
    /// </summary>
    public DateTime? LockoutEnd { get; set; }

    /// <summary>
    /// Whether the security tutorial has been completed (for children).
    /// </summary>
    public bool SecurityTutorialCompleted { get; set; }

    /// <summary>
    /// Last login timestamp.
    /// </summary>
    public DateTime? LastLoginAt { get; set; }

    // Navigation properties

    /// <summary>
    /// Family this child belongs to (only for children).
    /// </summary>
    public FamilyId? FamilyId { get; set; }
    public Family? Family { get; set; }

    /// <summary>
    /// Account for this user (only for children).
    /// </summary>
    public Account? Account { get; set; }

    /// <summary>
    /// Families where this user is a parent.
    /// </summary>
    public ICollection<FamilyParent> ParentFamilies { get; set; } = new List<FamilyParent>();

    /// <summary>
    /// Families where this user is a relative.
    /// </summary>
    public ICollection<FamilyRelative> RelativeFamilies { get; set; } = new List<FamilyRelative>();

    /// <summary>
    /// Passkeys registered for this user.
    /// </summary>
    public ICollection<Passkey> Passkeys { get; set; } = new List<Passkey>();

    /// <summary>
    /// Biometric tokens for this user.
    /// </summary>
    public ICollection<BiometricToken> BiometricTokens { get; set; } = new List<BiometricToken>();

    /// <summary>
    /// TOTP backup codes for this user.
    /// </summary>
    public ICollection<TotpBackupCode> TotpBackupCodes { get; set; } = new List<TotpBackupCode>();

    /// <summary>
    /// Active sessions for this user.
    /// </summary>
    public ICollection<Session> Sessions { get; set; } = new List<Session>();

    /// <summary>
    /// Login attempts for this user.
    /// </summary>
    public ICollection<LoginAttempt> LoginAttempts { get; set; } = new List<LoginAttempt>();
}
