using TaschengeldManager.Core.DTOs.Account;
using TaschengeldManager.Core.Entities;

namespace TaschengeldManager.Core.Interfaces.Services;

/// <summary>
/// Service interface for account operations.
/// </summary>
public interface IAccountService
{
    /// <summary>
    /// Get account by ID.
    /// </summary>
    Task<AccountDto?> GetAccountAsync(UserId userId, AccountId accountId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get account for current user (for children).
    /// </summary>
    Task<AccountDto?> GetMyAccountAsync(UserId userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get accounts for a family (for parents).
    /// </summary>
    Task<IReadOnlyList<AccountDto>> GetFamilyAccountsAsync(UserId parentUserId, FamilyId familyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deposit money into an account (by parent).
    /// </summary>
    Task<TransactionDto> DepositAsync(UserId parentUserId, AccountId accountId, DepositRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Withdraw/spend money from account (by child).
    /// </summary>
    Task<TransactionDto> WithdrawAsync(UserId userId, WithdrawRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gift money to a child (by relative).
    /// </summary>
    Task<TransactionDto> GiftAsync(UserId relativeUserId, GiftRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get transactions for an account.
    /// </summary>
    Task<IReadOnlyList<TransactionDto>> GetTransactionsAsync(UserId userId, AccountId accountId, int? limit = null, int? offset = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get transactions for the current user's account (for children).
    /// </summary>
    Task<IReadOnlyList<TransactionDto>> GetMyTransactionsAsync(UserId userId, int? limit = null, int? offset = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get own gifts (for relatives).
    /// </summary>
    Task<IReadOnlyList<TransactionDto>> GetMyGiftsAsync(UserId relativeUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Set interest settings for an account.
    /// </summary>
    Task<AccountDto> SetInterestAsync(UserId parentUserId, AccountId accountId, SetInterestRequest request, CancellationToken cancellationToken = default);
}
