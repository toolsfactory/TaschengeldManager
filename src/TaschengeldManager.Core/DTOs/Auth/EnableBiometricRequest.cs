using System.ComponentModel.DataAnnotations;
using TaschengeldManager.Core.Enums;

namespace TaschengeldManager.Core.DTOs.Auth;

/// <summary>
/// Request to enable biometric authentication.
/// </summary>
public record EnableBiometricRequest
{
    /// <summary>
    /// Unique device identifier.
    /// </summary>
    [Required]
    public required string DeviceId { get; init; }

    /// <summary>
    /// Device name for display.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public required string DeviceName { get; init; }

    /// <summary>
    /// Platform (iOS, Android).
    /// </summary>
    [Required]
    public required string Platform { get; init; }

    /// <summary>
    /// Type of biometry.
    /// </summary>
    [Required]
    public required BiometryType BiometryType { get; init; }
}
