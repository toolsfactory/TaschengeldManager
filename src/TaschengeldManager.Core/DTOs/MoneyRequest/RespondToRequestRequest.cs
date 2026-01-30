using System.ComponentModel.DataAnnotations;

namespace TaschengeldManager.Core.DTOs.MoneyRequest;

/// <summary>
/// Request to respond to a money request.
/// </summary>
public record RespondToRequestRequest
{
    /// <summary>
    /// Whether to approve the request.
    /// </summary>
    [Required]
    public bool Approve { get; init; }

    /// <summary>
    /// Optional note to the child (especially useful for rejections).
    /// </summary>
    [MaxLength(500)]
    public string? Note { get; init; }
}
