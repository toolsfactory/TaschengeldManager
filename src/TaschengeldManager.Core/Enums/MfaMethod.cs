namespace TaschengeldManager.Core.Enums;

/// <summary>
/// Available MFA methods.
/// </summary>
public enum MfaMethod
{
    /// <summary>
    /// Time-based One-Time Password (TOTP) via authenticator app.
    /// </summary>
    Totp = 0,

    /// <summary>
    /// Passkey (WebAuthn/FIDO2).
    /// </summary>
    Passkey = 1,

    /// <summary>
    /// Biometric authentication (Face ID, Touch ID, Fingerprint).
    /// </summary>
    Biometric = 2,

    /// <summary>
    /// Parent approval via push notification (for children only).
    /// </summary>
    ParentApproval = 3
}
