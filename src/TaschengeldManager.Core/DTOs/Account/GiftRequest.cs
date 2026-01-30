using System.ComponentModel.DataAnnotations;

namespace TaschengeldManager.Core.DTOs.Account;

/// <summary>
/// Request to gift money to a child (from relative).
/// </summary>
public record GiftRequest
{
    /// <summary>
    /// Child's account ID.
    /// </summary>
    [Required]
    public required Guid AccountId { get; init; }

    /// <summary>
    /// Amount to gift.
    /// </summary>
    [Required]
    [Range(0.01, 10000)]
    public required decimal Amount { get; init; }

    /// <summary>
    /// Message/occasion.
    /// </summary>
    [MaxLength(200)]
    public string? Description { get; init; }
}
