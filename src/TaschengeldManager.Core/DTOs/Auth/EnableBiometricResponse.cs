namespace TaschengeldManager.Core.DTOs.Auth;

/// <summary>
/// Response after enabling biometric authentication.
/// </summary>
public record EnableBiometricResponse
{
    /// <summary>
    /// Biometric token to store securely on device.
    /// </summary>
    public required string BiometricToken { get; init; }

    /// <summary>
    /// When the token expires.
    /// </summary>
    public required DateTime ExpiresAt { get; init; }
}
