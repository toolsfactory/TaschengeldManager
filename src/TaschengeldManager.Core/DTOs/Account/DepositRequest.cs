using System.ComponentModel.DataAnnotations;

namespace TaschengeldManager.Core.DTOs.Account;

/// <summary>
/// Request to deposit money into an account.
/// </summary>
public record DepositRequest
{
    /// <summary>
    /// Amount to deposit.
    /// </summary>
    [Required]
    [Range(0.01, 10000)]
    public required decimal Amount { get; init; }

    /// <summary>
    /// Description of the deposit.
    /// </summary>
    [MaxLength(200)]
    public string? Description { get; init; }
}
