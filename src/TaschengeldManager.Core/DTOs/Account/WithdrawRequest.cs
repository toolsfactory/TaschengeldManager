using System.ComponentModel.DataAnnotations;

namespace TaschengeldManager.Core.DTOs.Account;

/// <summary>
/// Request to withdraw/spend money from an account.
/// </summary>
public record WithdrawRequest
{
    /// <summary>
    /// Amount to withdraw.
    /// </summary>
    [Required]
    [Range(0.01, 10000)]
    public required decimal Amount { get; init; }

    /// <summary>
    /// Description of what the money was spent on.
    /// </summary>
    [MaxLength(200)]
    public string? Description { get; init; }

    /// <summary>
    /// Category of the expense.
    /// </summary>
    [MaxLength(50)]
    public string? Category { get; init; }
}
