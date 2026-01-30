using System.ComponentModel.DataAnnotations;

namespace TaschengeldManager.Core.DTOs.Family;

/// <summary>
/// Request to add a child to the family.
/// </summary>
public record AddChildRequest
{
    /// <summary>
    /// Child's nickname.
    /// </summary>
    [Required]
    [MinLength(2)]
    [MaxLength(50)]
    public required string Nickname { get; init; }

    /// <summary>
    /// Child's PIN for login.
    /// </summary>
    [Required]
    [StringLength(6, MinimumLength = 4)]
    public required string Pin { get; init; }

    /// <summary>
    /// Initial balance for the account.
    /// </summary>
    [Range(0, 10000)]
    public decimal InitialBalance { get; init; } = 0;
}
