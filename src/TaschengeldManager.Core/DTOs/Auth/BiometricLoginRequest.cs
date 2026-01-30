using System.ComponentModel.DataAnnotations;

namespace TaschengeldManager.Core.DTOs.Auth;

/// <summary>
/// Request to login with biometric token.
/// </summary>
public record BiometricLoginRequest
{
    /// <summary>
    /// Device identifier.
    /// </summary>
    [Required]
    public required string DeviceId { get; init; }

    /// <summary>
    /// Biometric token.
    /// </summary>
    [Required]
    public required string BiometricToken { get; init; }
}
