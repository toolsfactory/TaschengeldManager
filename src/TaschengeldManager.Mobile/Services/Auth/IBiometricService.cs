namespace TaschengeldManager.Mobile.Services.Auth;

/// <summary>
/// Service for handling device-level biometric authentication (fingerprint/face)
/// </summary>
public interface IBiometricService
{
    /// <summary>
    /// Check if biometric authentication is available on this device
    /// </summary>
    Task<BiometricAvailability> GetAvailabilityAsync();

    /// <summary>
    /// Prompt the user to authenticate using biometrics
    /// </summary>
    Task<BiometricAuthResult> AuthenticateAsync(string reason, CancellationToken ct = default);
}

/// <summary>
/// Result of biometric availability check
/// </summary>
public enum BiometricAvailability
{
    /// <summary>
    /// Biometrics available and enrolled
    /// </summary>
    Available,

    /// <summary>
    /// Device doesn't support biometrics
    /// </summary>
    NoHardware,

    /// <summary>
    /// Biometrics supported but not enrolled
    /// </summary>
    NotEnrolled,

    /// <summary>
    /// Biometrics temporarily unavailable (e.g., too many failed attempts)
    /// </summary>
    TemporarilyUnavailable,

    /// <summary>
    /// Permission denied by user
    /// </summary>
    PermissionDenied,

    /// <summary>
    /// Unknown error
    /// </summary>
    Unknown
}

/// <summary>
/// Result of a biometric authentication attempt
/// </summary>
public record BiometricAuthResult
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public BiometricAuthStatus Status { get; init; }

    public static BiometricAuthResult Succeeded() => new() { Success = true, Status = BiometricAuthStatus.Succeeded };
    public static BiometricAuthResult Failed(BiometricAuthStatus status, string? message = null) => new() { Status = status, ErrorMessage = message };
}

/// <summary>
/// Status of biometric authentication
/// </summary>
public enum BiometricAuthStatus
{
    Succeeded,
    Failed,
    Cancelled,
    TooManyAttempts,
    NotAvailable,
    Unknown
}
