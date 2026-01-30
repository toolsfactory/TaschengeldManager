namespace TaschengeldManager.Core.DTOs.Family;

/// <summary>
/// Child information for display.
/// </summary>
public record ChildDto
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
    /// Account ID.
    /// </summary>
    public Guid? AccountId { get; init; }

    /// <summary>
    /// Current balance.
    /// </summary>
    public decimal? Balance { get; init; }

    /// <summary>
    /// Whether the account is locked.
    /// </summary>
    public bool IsLocked { get; init; }

    /// <summary>
    /// Whether MFA is set up.
    /// </summary>
    public bool MfaEnabled { get; init; }

    /// <summary>
    /// When the child was added.
    /// </summary>
    public required DateTime CreatedAt { get; init; }
}
