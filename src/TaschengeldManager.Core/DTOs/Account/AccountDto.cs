namespace TaschengeldManager.Core.DTOs.Account;

/// <summary>
/// Account information for display.
/// </summary>
public record AccountDto
{
    /// <summary>
    /// Account ID.
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// Owner's user ID.
    /// </summary>
    public required Guid UserId { get; init; }

    /// <summary>
    /// Owner's name.
    /// </summary>
    public required string OwnerName { get; init; }

    /// <summary>
    /// Current balance in EUR.
    /// </summary>
    public required decimal Balance { get; init; }

    /// <summary>
    /// Whether interest is enabled.
    /// </summary>
    public bool InterestEnabled { get; init; }

    /// <summary>
    /// Interest rate (if enabled).
    /// </summary>
    public decimal? InterestRate { get; init; }

    /// <summary>
    /// When the account was created.
    /// </summary>
    public required DateTime CreatedAt { get; init; }
}
