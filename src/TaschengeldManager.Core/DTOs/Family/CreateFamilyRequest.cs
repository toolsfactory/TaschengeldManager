using System.ComponentModel.DataAnnotations;

namespace TaschengeldManager.Core.DTOs.Family;

/// <summary>
/// Request to create a new family.
/// </summary>
public record CreateFamilyRequest
{
    /// <summary>
    /// Name of the family.
    /// </summary>
    [Required]
    [MinLength(2)]
    [MaxLength(100)]
    public required string Name { get; init; }
}
