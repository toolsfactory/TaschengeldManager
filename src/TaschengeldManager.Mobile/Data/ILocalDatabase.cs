using TaschengeldManager.Mobile.Data.Entities;

namespace TaschengeldManager.Mobile.Data;

/// <summary>
/// Interface for local SQLite database operations
/// </summary>
public interface ILocalDatabase
{
    /// <summary>
    /// Initialize the database and create tables
    /// </summary>
    Task InitializeAsync();

    /// <summary>
    /// Clear all cached data (but keep pending operations)
    /// </summary>
    Task ClearCacheAsync();

    /// <summary>
    /// Clear everything including pending operations
    /// </summary>
    Task ClearAllAsync();

    // Account operations
    Task<CachedAccount?> GetAccountAsync(Guid id);
    Task<CachedAccount?> GetAccountByUserIdAsync(Guid userId);
    Task<List<CachedAccount>> GetAccountsByFamilyAsync(Guid familyId);
    Task SaveAccountAsync(CachedAccount account);
    Task SaveAccountsAsync(IEnumerable<CachedAccount> accounts);

    // Transaction operations
    Task<CachedTransaction?> GetTransactionAsync(Guid id);
    Task<List<CachedTransaction>> GetTransactionsByAccountAsync(Guid accountId, int? limit = null);
    Task SaveTransactionAsync(CachedTransaction transaction);
    Task SaveTransactionsAsync(IEnumerable<CachedTransaction> transactions);

    // Family operations
    Task<CachedFamily?> GetFamilyAsync(Guid id);
    Task<List<CachedFamily>> GetAllFamiliesAsync();
    Task SaveFamilyAsync(CachedFamily family);

    // Child operations
    Task<CachedChild?> GetChildAsync(Guid id);
    Task<List<CachedChild>> GetChildrenByFamilyAsync(Guid familyId);
    Task SaveChildAsync(CachedChild child);
    Task SaveChildrenAsync(IEnumerable<CachedChild> children);

    // Pending operations
    Task<List<PendingOperation>> GetPendingOperationsAsync();
    Task AddPendingOperationAsync(PendingOperation operation);
    Task UpdatePendingOperationAsync(PendingOperation operation);
    Task RemovePendingOperationAsync(int id);
    Task<int> GetPendingOperationsCountAsync();
}
