using TaschengeldManager.Core.Entities;
using TaschengeldManager.Core.Enums;

namespace TaschengeldManager.Core.Interfaces.Repositories;

/// <summary>
/// Repository interface for Transaction entities.
/// </summary>
public interface ITransactionRepository : IRepository<Transaction, TransactionId>
{
    /// <summary>
    /// Get transactions for an account.
    /// </summary>
    Task<IReadOnlyList<Transaction>> GetByAccountAsync(AccountId accountId, int? limit = null, int? offset = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get transactions by a specific user (for relatives to see their gifts).
    /// </summary>
    Task<IReadOnlyList<Transaction>> GetByCreatorAsync(UserId userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get transactions by type.
    /// </summary>
    Task<IReadOnlyList<Transaction>> GetByTypeAsync(AccountId accountId, TransactionType type, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get transactions in date range.
    /// </summary>
    Task<IReadOnlyList<Transaction>> GetByDateRangeAsync(AccountId accountId, DateTime from, DateTime to, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get transactions for multiple accounts in date range (for family statistics).
    /// </summary>
    Task<IReadOnlyList<Transaction>> GetByAccountsAndDateRangeAsync(IEnumerable<AccountId> accountIds, DateTime from, DateTime to, CancellationToken cancellationToken = default);
}
