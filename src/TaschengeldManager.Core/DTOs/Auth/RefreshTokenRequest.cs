using System.ComponentModel.DataAnnotations;

namespace TaschengeldManager.Core.DTOs.Auth;

/// <summary>
/// Request to refresh access token.
/// </summary>
public record RefreshTokenRequest
{
    /// <summary>
    /// Current refresh token.
    /// </summary>
    [Required]
    public required string RefreshToken { get; init; }
}
