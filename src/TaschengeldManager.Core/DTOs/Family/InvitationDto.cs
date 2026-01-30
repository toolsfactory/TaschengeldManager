using TaschengeldManager.Core.Enums;

namespace TaschengeldManager.Core.DTOs.Family;

/// <summary>
/// Invitation information for display.
/// </summary>
public record InvitationDto
{
    /// <summary>
    /// Invitation ID.
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// Invited email address.
    /// </summary>
    public required string InvitedEmail { get; init; }

    /// <summary>
    /// Invited role.
    /// </summary>
    public required UserRole InvitedRole { get; init; }

    /// <summary>
    /// Current status.
    /// </summary>
    public required InvitationStatus Status { get; init; }

    /// <summary>
    /// Who sent the invitation.
    /// </summary>
    public required string InvitedByName { get; init; }

    /// <summary>
    /// When the invitation was created.
    /// </summary>
    public required DateTime CreatedAt { get; init; }

    /// <summary>
    /// When the invitation expires.
    /// </summary>
    public required DateTime ExpiresAt { get; init; }

    /// <summary>
    /// Relationship description (for relatives).
    /// </summary>
    public string? RelationshipDescription { get; init; }
}
