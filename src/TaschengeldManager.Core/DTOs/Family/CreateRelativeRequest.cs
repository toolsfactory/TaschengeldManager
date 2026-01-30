using System.ComponentModel.DataAnnotations;

namespace TaschengeldManager.Core.DTOs.Family;

/// <summary>
/// Request to create a relative account (by parent).
/// </summary>
public record CreateRelativeRequest
{
    /// <summary>
    /// Email address.
    /// </summary>
    [Required]
    [EmailAddress]
    public required string Email { get; init; }

    /// <summary>
    /// Display name.
    /// </summary>
    [Required]
    [MinLength(2)]
    [MaxLength(50)]
    public required string Nickname { get; init; }

    /// <summary>
    /// Temporary password.
    /// </summary>
    [Required]
    [MinLength(8)]
    public required string Password { get; init; }

    /// <summary>
    /// Relationship description.
    /// </summary>
    [MaxLength(50)]
    public string? RelationshipDescription { get; init; }
}
