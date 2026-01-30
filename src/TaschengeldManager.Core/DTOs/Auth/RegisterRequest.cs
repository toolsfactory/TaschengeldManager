using System.ComponentModel.DataAnnotations;

namespace TaschengeldManager.Core.DTOs.Auth;

/// <summary>
/// Request to register a new parent user.
/// </summary>
public record RegisterRequest
{
    /// <summary>
    /// Email address.
    /// </summary>
    [Required]
    [EmailAddress]
    [MaxLength(256)]
    public required string Email { get; init; }

    /// <summary>
    /// Password (min 8 characters).
    /// </summary>
    [Required]
    [MinLength(8)]
    [MaxLength(128)]
    public required string Password { get; init; }

    /// <summary>
    /// Display name/nickname.
    /// </summary>
    [Required]
    [MinLength(2)]
    [MaxLength(50)]
    public required string Nickname { get; init; }
}
