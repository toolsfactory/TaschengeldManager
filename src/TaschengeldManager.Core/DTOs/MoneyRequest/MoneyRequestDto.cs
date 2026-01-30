using TaschengeldManager.Core.Enums;

namespace TaschengeldManager.Core.DTOs.MoneyRequest;

/// <summary>
/// DTO for money request information.
/// </summary>
public record MoneyRequestDto
{
    /// <summary>
    /// Unique identifier.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Name of the child making the request.
    /// </summary>
    public string ChildName { get; init; } = string.Empty;

    /// <summary>
    /// User ID of the child.
    /// </summary>
    public Guid ChildUserId { get; init; }

    /// <summary>
    /// Requested amount.
    /// </summary>
    public decimal Amount { get; init; }

    /// <summary>
    /// Reason for the request.
    /// </summary>
    public string Reason { get; init; } = string.Empty;

    /// <summary>
    /// Current status.
    /// </summary>
    public RequestStatus Status { get; init; }

    /// <summary>
    /// Response note from parent (if responded).
    /// </summary>
    public string? ResponseNote { get; init; }

    /// <summary>
    /// Name of the parent who responded (if responded).
    /// </summary>
    public string? RespondedByName { get; init; }

    /// <summary>
    /// When the request was responded to.
    /// </summary>
    public DateTime? RespondedAt { get; init; }

    /// <summary>
    /// When the request was created.
    /// </summary>
    public DateTime CreatedAt { get; init; }
}
