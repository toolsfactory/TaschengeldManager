using System.ComponentModel.DataAnnotations;

namespace TaschengeldManager.Core.DTOs.Auth;

/// <summary>
/// Request to login with email and password.
/// </summary>
public record LoginRequest
{
    /// <summary>
    /// Email address.
    /// </summary>
    [Required]
    [EmailAddress]
    public required string Email { get; init; }

    /// <summary>
    /// Password.
    /// </summary>
    [Required]
    public required string Password { get; init; }
}
