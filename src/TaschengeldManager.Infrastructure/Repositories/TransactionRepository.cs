using Microsoft.EntityFrameworkCore;
using TaschengeldManager.Core.Entities;
using TaschengeldManager.Core.Enums;
using TaschengeldManager.Core.Interfaces.Repositories;
using TaschengeldManager.Infrastructure.Data;

namespace TaschengeldManager.Infrastructure.Repositories;

public class TransactionRepository : Repository<Transaction, TransactionId>, ITransactionRepository
{
    public TransactionRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Transaction>> GetByAccountAsync(AccountId accountId, int? limit = null, int? offset = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Where(t => t.AccountId == accountId)
            .Include(t => t.CreatedByUser)
            .OrderByDescending(t => t.CreatedAt)
            .AsQueryable();

        if (offset.HasValue)
            query = query.Skip(offset.Value);

        if (limit.HasValue)
            query = query.Take(limit.Value);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Transaction>> GetByCreatorAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.CreatedByUserId == userId)
            .Include(t => t.Account)
                .ThenInclude(a => a!.User)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Transaction>> GetByTypeAsync(AccountId accountId, TransactionType type, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.AccountId == accountId && t.Type == type)
            .Include(t => t.CreatedByUser)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Transaction>> GetByDateRangeAsync(AccountId accountId, DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.AccountId == accountId && t.CreatedAt >= from && t.CreatedAt <= to)
            .Include(t => t.CreatedByUser)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Transaction>> GetByAccountsAndDateRangeAsync(IEnumerable<AccountId> accountIds, DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        var accountIdList = accountIds.ToList();
        return await _dbSet
            .Where(t => accountIdList.Contains(t.AccountId) && t.CreatedAt >= from && t.CreatedAt <= to)
            .Include(t => t.CreatedByUser)
            .Include(t => t.Account)
                .ThenInclude(a => a!.User)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
