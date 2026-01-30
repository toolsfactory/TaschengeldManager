using TaschengeldManager.Core.Enums;

namespace TaschengeldManager.Core.Entities;

/// <summary>
/// Represents a child's allowance account.
/// </summary>
public class Account : BaseEntity<AccountId>
{
    /// <summary>
    /// Current balance in EUR (2 decimal places).
    /// </summary>
    public decimal Balance { get; set; }

    /// <summary>
    /// User (child) this account belongs to.
    /// </summary>
    public UserId UserId { get; set; }
    public User? User { get; set; }

    /// <summary>
    /// Whether interest is enabled for this account.
    /// </summary>
    public bool InterestEnabled { get; set; }

    /// <summary>
    /// Annual interest rate in percent (e.g., 2.5 for 2.5%).
    /// </summary>
    public decimal? InterestRate { get; set; }

    /// <summary>
    /// Interest calculation and crediting interval.
    /// </summary>
    public InterestInterval InterestInterval { get; set; } = InterestInterval.Monthly;

    /// <summary>
    /// Last date interest was calculated.
    /// </summary>
    public DateTime? LastInterestCalculation { get; set; }

    /// <summary>
    /// Transactions on this account.
    /// </summary>
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
