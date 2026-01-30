using TaschengeldManager.Core.DTOs.Auth;
using TaschengeldManager.Core.Entities;

namespace TaschengeldManager.Core.Interfaces.Services;

/// <summary>
/// Service interface for MFA operations.
/// </summary>
public interface IMfaService
{
    /// <summary>
    /// Setup TOTP for a user.
    /// </summary>
    Task<SetupTotpResponse> SetupTotpAsync(UserId userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verify and activate TOTP.
    /// </summary>
    Task<bool> VerifyAndActivateTotpAsync(UserId userId, string setupToken, string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verify TOTP code.
    /// </summary>
    Task<bool> VerifyTotpAsync(UserId userId, string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generate backup codes.
    /// </summary>
    Task<BackupCodesResponse> GenerateBackupCodesAsync(UserId userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verify backup code.
    /// </summary>
    Task<bool> VerifyBackupCodeAsync(UserId userId, string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Enable biometric authentication.
    /// </summary>
    Task<EnableBiometricResponse> EnableBiometricAsync(UserId userId, EnableBiometricRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Login with biometric token.
    /// </summary>
    Task<LoginResponse?> BiometricLoginAsync(BiometricLoginRequest request, string? ipAddress, string? userAgent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Disable biometric authentication.
    /// </summary>
    Task DisableBiometricAsync(UserId userId, string deviceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get biometric status for a device.
    /// </summary>
    Task<EnableBiometricResponse?> GetBiometricStatusAsync(UserId userId, string deviceId, CancellationToken cancellationToken = default);
}
