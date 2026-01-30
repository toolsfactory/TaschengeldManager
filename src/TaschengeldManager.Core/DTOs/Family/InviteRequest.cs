using System.ComponentModel.DataAnnotations;
using TaschengeldManager.Core.Enums;

namespace TaschengeldManager.Core.DTOs.Family;

/// <summary>
/// Request to invite someone to the family.
/// </summary>
public record InviteRequest
{
    /// <summary>
    /// Email address of the person to invite.
    /// </summary>
    [Required]
    [EmailAddress]
    public required string Email { get; init; }

    /// <summary>
    /// Role to invite as (Parent or Relative).
    /// </summary>
    [Required]
    public required UserRole Role { get; init; }

    /// <summary>
    /// Relationship description (for relatives).
    /// </summary>
    [MaxLength(50)]
    public string? RelationshipDescription { get; init; }
}
