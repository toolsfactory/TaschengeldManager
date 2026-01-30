using Microsoft.EntityFrameworkCore;
using TaschengeldManager.Core.Entities;
using TaschengeldManager.Core.Interfaces.Repositories;
using TaschengeldManager.Infrastructure.Data;

namespace TaschengeldManager.Infrastructure.Repositories;

public class AccountRepository : Repository<Account, AccountId>, IAccountRepository
{
    public AccountRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Account?> GetByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.UserId == userId, cancellationToken);
    }

    public async Task<Account?> GetWithTransactionsAsync(AccountId accountId, int? limit = null, CancellationToken cancellationToken = default)
    {
        if (limit.HasValue)
        {
            // Use filtered include with limit
            return await _dbSet
                .Include(a => a.User)
                .Include(a => a.Transactions.OrderByDescending(t => t.CreatedAt).Take(limit.Value))
                .Where(a => a.Id == accountId)
                .FirstOrDefaultAsync(cancellationToken);
        }

        return await _dbSet
            .Include(a => a.User)
            .Include(a => a.Transactions.OrderByDescending(t => t.CreatedAt))
            .Where(a => a.Id == accountId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Account>> GetByFamilyAsync(FamilyId familyId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.User)
            .Where(a => a.User != null && a.User.FamilyId == familyId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Account>> GetWithInterestEnabledAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.User)
            .Where(a => a.InterestEnabled && a.InterestRate != null && a.InterestRate > 0)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Account>> GetByUserIdsAsync(IEnumerable<UserId> userIds, CancellationToken cancellationToken = default)
    {
        var userIdList = userIds.ToList();
        if (userIdList.Count == 0)
        {
            return [];
        }

        return await _dbSet
            .Include(a => a.User)
            .Where(a => userIdList.Contains(a.UserId))
            .ToListAsync(cancellationToken);
    }
}
