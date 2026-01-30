using TaschengeldManager.Core.Enums;

namespace TaschengeldManager.Core.DTOs.Family;

/// <summary>
/// Family member (parent or relative) information.
/// </summary>
public record FamilyMemberDto
{
    /// <summary>
    /// User ID.
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// Display name.
    /// </summary>
    public required string Nickname { get; init; }

    /// <summary>
    /// Email address.
    /// </summary>
    public string? Email { get; init; }

    /// <summary>
    /// Role (Parent or Relative).
    /// </summary>
    public required UserRole Role { get; init; }

    /// <summary>
    /// When they joined the family.
    /// </summary>
    public required DateTime JoinedAt { get; init; }

    /// <summary>
    /// Whether this is the primary parent (creator).
    /// </summary>
    public bool IsPrimary { get; init; }

    /// <summary>
    /// Relationship description (for relatives).
    /// </summary>
    public string? RelationshipDescription { get; init; }
}
