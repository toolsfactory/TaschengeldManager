using TaschengeldManager.Core.Enums;

namespace TaschengeldManager.Core.DTOs.Auth;

/// <summary>
/// User information for display.
/// </summary>
public record UserDto
{
    /// <summary>
    /// User ID.
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// Email address (null for children).
    /// </summary>
    public string? Email { get; init; }

    /// <summary>
    /// Display name.
    /// </summary>
    public required string Nickname { get; init; }

    /// <summary>
    /// User role.
    /// </summary>
    public required UserRole Role { get; init; }

    /// <summary>
    /// Whether MFA is enabled.
    /// </summary>
    public bool MfaEnabled { get; init; }

    /// <summary>
    /// Family ID (for children).
    /// </summary>
    public Guid? FamilyId { get; init; }

    /// <summary>
    /// Whether security tutorial was completed (for children).
    /// </summary>
    public bool SecurityTutorialCompleted { get; init; }
}
