using System.ComponentModel.DataAnnotations;

namespace TaschengeldManager.Core.DTOs.Family;

/// <summary>
/// Request to change a child's PIN.
/// </summary>
public record ChangeChildPinRequest
{
    /// <summary>
    /// The new PIN for the child.
    /// </summary>
    [Required]
    [StringLength(6, MinimumLength = 4)]
    public required string NewPin { get; init; }

    /// <summary>
    /// Parent's password for verification.
    /// </summary>
    [Required]
    public required string ParentPassword { get; init; }
}
