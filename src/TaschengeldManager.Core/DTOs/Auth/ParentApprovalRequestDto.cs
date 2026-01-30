using TaschengeldManager.Core.Enums;

namespace TaschengeldManager.Core.DTOs.Auth;

/// <summary>
/// Parent approval request for display.
/// </summary>
public record ParentApprovalRequestDto
{
    /// <summary>
    /// Request ID.
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// Child's name.
    /// </summary>
    public required string ChildName { get; init; }

    /// <summary>
    /// Device information.
    /// </summary>
    public string? DeviceInfo { get; init; }

    /// <summary>
    /// Approximate location.
    /// </summary>
    public string? ApproximateLocation { get; init; }

    /// <summary>
    /// Current status.
    /// </summary>
    public required ParentApprovalStatus Status { get; init; }

    /// <summary>
    /// When created.
    /// </summary>
    public required DateTime CreatedAt { get; init; }

    /// <summary>
    /// When it expires.
    /// </summary>
    public required DateTime ExpiresAt { get; init; }
}
