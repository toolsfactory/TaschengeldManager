using System.ComponentModel.DataAnnotations;

namespace TaschengeldManager.Core.DTOs.Auth;

/// <summary>
/// Request for child login.
/// </summary>
public record ChildLoginRequest
{
    /// <summary>
    /// Family code (6 characters).
    /// </summary>
    [Required]
    [StringLength(6, MinimumLength = 6)]
    public required string FamilyCode { get; init; }

    /// <summary>
    /// Child's nickname.
    /// </summary>
    [Required]
    [MaxLength(50)]
    public required string Nickname { get; init; }

    /// <summary>
    /// Child's PIN.
    /// </summary>
    [Required]
    [StringLength(6, MinimumLength = 4)]
    public required string Pin { get; init; }
}
