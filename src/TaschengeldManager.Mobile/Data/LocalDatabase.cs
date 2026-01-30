using SQLite;
using TaschengeldManager.Mobile.Data.Entities;

namespace TaschengeldManager.Mobile.Data;

/// <summary>
/// SQLite database implementation for local caching
/// </summary>
public class LocalDatabase : ILocalDatabase
{
    private SQLiteAsyncConnection? _database;
    private readonly SemaphoreSlim _initLock = new(1, 1);
    private bool _initialized;

    private const string DatabaseName = "taschengeld_cache.db3";

    private static string DatabasePath =>
        Path.Combine(FileSystem.AppDataDirectory, DatabaseName);

    private async Task<SQLiteAsyncConnection> GetDatabaseAsync()
    {
        if (_initialized && _database != null)
            return _database;

        await _initLock.WaitAsync();
        try
        {
            if (_initialized && _database != null)
                return _database;

            await InitializeAsync();
            return _database!;
        }
        finally
        {
            _initLock.Release();
        }
    }

    public async Task InitializeAsync()
    {
        if (_initialized)
            return;

        _database = new SQLiteAsyncConnection(DatabasePath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);

        // Create tables
        await _database.CreateTableAsync<CachedAccount>();
        await _database.CreateTableAsync<CachedTransaction>();
        await _database.CreateTableAsync<CachedFamily>();
        await _database.CreateTableAsync<CachedChild>();
        await _database.CreateTableAsync<PendingOperation>();

        _initialized = true;
    }

    public async Task ClearCacheAsync()
    {
        var db = await GetDatabaseAsync();
        await db.DeleteAllAsync<CachedAccount>();
        await db.DeleteAllAsync<CachedTransaction>();
        await db.DeleteAllAsync<CachedFamily>();
        await db.DeleteAllAsync<CachedChild>();
    }

    public async Task ClearAllAsync()
    {
        await ClearCacheAsync();
        var db = await GetDatabaseAsync();
        await db.DeleteAllAsync<PendingOperation>();
    }

    #region Account Operations

    public async Task<CachedAccount?> GetAccountAsync(Guid id)
    {
        var db = await GetDatabaseAsync();
        return await db.Table<CachedAccount>()
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<CachedAccount?> GetAccountByUserIdAsync(Guid userId)
    {
        var db = await GetDatabaseAsync();
        return await db.Table<CachedAccount>()
            .FirstOrDefaultAsync(a => a.UserId == userId);
    }

    public async Task<List<CachedAccount>> GetAccountsByFamilyAsync(Guid familyId)
    {
        var db = await GetDatabaseAsync();
        var children = await db.Table<CachedChild>()
            .Where(c => c.FamilyId == familyId)
            .ToListAsync();

        var accountIds = children.Select(c => c.AccountId).ToList();
        var accounts = await db.Table<CachedAccount>().ToListAsync();
        return accounts.Where(a => accountIds.Contains(a.Id)).ToList();
    }

    public async Task SaveAccountAsync(CachedAccount account)
    {
        account.CachedAt = DateTime.UtcNow;
        var db = await GetDatabaseAsync();
        await db.InsertOrReplaceAsync(account);
    }

    public async Task SaveAccountsAsync(IEnumerable<CachedAccount> accounts)
    {
        var now = DateTime.UtcNow;
        var list = accounts.ToList();
        foreach (var account in list)
        {
            account.CachedAt = now;
        }

        var db = await GetDatabaseAsync();
        await db.RunInTransactionAsync(conn =>
        {
            foreach (var account in list)
            {
                conn.InsertOrReplace(account);
            }
        });
    }

    #endregion

    #region Transaction Operations

    public async Task<CachedTransaction?> GetTransactionAsync(Guid id)
    {
        var db = await GetDatabaseAsync();
        return await db.Table<CachedTransaction>()
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<List<CachedTransaction>> GetTransactionsByAccountAsync(Guid accountId, int? limit = null)
    {
        var db = await GetDatabaseAsync();
        var query = db.Table<CachedTransaction>()
            .Where(t => t.AccountId == accountId)
            .OrderByDescending(t => t.CreatedAt);

        if (limit.HasValue)
        {
            return await query.Take(limit.Value).ToListAsync();
        }

        return await query.ToListAsync();
    }

    public async Task SaveTransactionAsync(CachedTransaction transaction)
    {
        transaction.CachedAt = DateTime.UtcNow;
        var db = await GetDatabaseAsync();
        await db.InsertOrReplaceAsync(transaction);
    }

    public async Task SaveTransactionsAsync(IEnumerable<CachedTransaction> transactions)
    {
        var now = DateTime.UtcNow;
        var list = transactions.ToList();
        foreach (var transaction in list)
        {
            transaction.CachedAt = now;
        }

        var db = await GetDatabaseAsync();
        await db.RunInTransactionAsync(conn =>
        {
            foreach (var transaction in list)
            {
                conn.InsertOrReplace(transaction);
            }
        });
    }

    #endregion

    #region Family Operations

    public async Task<CachedFamily?> GetFamilyAsync(Guid id)
    {
        var db = await GetDatabaseAsync();
        return await db.Table<CachedFamily>()
            .FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<List<CachedFamily>> GetAllFamiliesAsync()
    {
        var db = await GetDatabaseAsync();
        return await db.Table<CachedFamily>().ToListAsync();
    }

    public async Task SaveFamilyAsync(CachedFamily family)
    {
        family.CachedAt = DateTime.UtcNow;
        var db = await GetDatabaseAsync();
        await db.InsertOrReplaceAsync(family);
    }

    #endregion

    #region Child Operations

    public async Task<CachedChild?> GetChildAsync(Guid id)
    {
        var db = await GetDatabaseAsync();
        return await db.Table<CachedChild>()
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<List<CachedChild>> GetChildrenByFamilyAsync(Guid familyId)
    {
        var db = await GetDatabaseAsync();
        return await db.Table<CachedChild>()
            .Where(c => c.FamilyId == familyId)
            .ToListAsync();
    }

    public async Task SaveChildAsync(CachedChild child)
    {
        child.CachedAt = DateTime.UtcNow;
        var db = await GetDatabaseAsync();
        await db.InsertOrReplaceAsync(child);
    }

    public async Task SaveChildrenAsync(IEnumerable<CachedChild> children)
    {
        var now = DateTime.UtcNow;
        var list = children.ToList();
        foreach (var child in list)
        {
            child.CachedAt = now;
        }

        var db = await GetDatabaseAsync();
        await db.RunInTransactionAsync(conn =>
        {
            foreach (var child in list)
            {
                conn.InsertOrReplace(child);
            }
        });
    }

    #endregion

    #region Pending Operations

    public async Task<List<PendingOperation>> GetPendingOperationsAsync()
    {
        var db = await GetDatabaseAsync();
        return await db.Table<PendingOperation>()
            .OrderBy(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task AddPendingOperationAsync(PendingOperation operation)
    {
        operation.CreatedAt = DateTime.UtcNow;
        var db = await GetDatabaseAsync();
        await db.InsertAsync(operation);
    }

    public async Task UpdatePendingOperationAsync(PendingOperation operation)
    {
        var db = await GetDatabaseAsync();
        await db.UpdateAsync(operation);
    }

    public async Task RemovePendingOperationAsync(int id)
    {
        var db = await GetDatabaseAsync();
        await db.DeleteAsync<PendingOperation>(id);
    }

    public async Task<int> GetPendingOperationsCountAsync()
    {
        var db = await GetDatabaseAsync();
        return await db.Table<PendingOperation>().CountAsync();
    }

    #endregion
}
