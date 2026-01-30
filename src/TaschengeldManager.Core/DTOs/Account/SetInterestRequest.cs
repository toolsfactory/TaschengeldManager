using System.ComponentModel.DataAnnotations;

namespace TaschengeldManager.Core.DTOs.Account;

/// <summary>
/// Request to set interest settings for an account.
/// </summary>
public record SetInterestRequest
{
    /// <summary>
    /// Whether interest is enabled.
    /// </summary>
    [Required]
    public required bool Enabled { get; init; }

    /// <summary>
    /// Annual interest rate in percent.
    /// </summary>
    [Range(0, 100)]
    public decimal? InterestRate { get; init; }
}
