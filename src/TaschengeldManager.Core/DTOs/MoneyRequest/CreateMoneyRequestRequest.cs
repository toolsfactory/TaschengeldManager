using System.ComponentModel.DataAnnotations;

namespace TaschengeldManager.Core.DTOs.MoneyRequest;

/// <summary>
/// Request to create a new money request.
/// </summary>
public record CreateMoneyRequestRequest
{
    /// <summary>
    /// Requested amount.
    /// </summary>
    [Required]
    [Range(0.01, 10000)]
    public decimal Amount { get; init; }

    /// <summary>
    /// Reason for the request.
    /// </summary>
    [Required]
    [MinLength(4)]
    [MaxLength(500)]
    public required string Reason { get; init; }
}
