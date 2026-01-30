using TaschengeldManager.Core.Entities;

namespace TaschengeldManager.Core.Interfaces.Repositories;

/// <summary>
/// Repository interface for Account entities.
/// </summary>
public interface IAccountRepository : IRepository<Account, AccountId>
{
    /// <summary>
    /// Get account by user ID.
    /// </summary>
    Task<Account?> GetByUserIdAsync(UserId userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get account with transactions.
    /// </summary>
    Task<Account?> GetWithTransactionsAsync(AccountId accountId, int? limit = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get accounts for a family (all children).
    /// </summary>
    Task<IReadOnlyList<Account>> GetByFamilyAsync(FamilyId familyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all accounts with interest enabled.
    /// </summary>
    Task<IReadOnlyList<Account>> GetWithInterestEnabledAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get accounts by multiple user IDs (batch loading to avoid N+1).
    /// </summary>
    Task<IReadOnlyList<Account>> GetByUserIdsAsync(IEnumerable<UserId> userIds, CancellationToken cancellationToken = default);
}
