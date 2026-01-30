using System.ComponentModel.DataAnnotations;

namespace TaschengeldManager.Core.DTOs.Auth;

/// <summary>
/// Request to verify TOTP code.
/// </summary>
public record VerifyTotpRequest
{
    /// <summary>
    /// MFA token from login response.
    /// </summary>
    [Required]
    public required string MfaToken { get; init; }

    /// <summary>
    /// TOTP code from authenticator app.
    /// </summary>
    [Required]
    [StringLength(6, MinimumLength = 4)]
    public required string Code { get; init; }
}
