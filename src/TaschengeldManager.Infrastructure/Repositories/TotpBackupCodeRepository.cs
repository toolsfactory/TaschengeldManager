using Microsoft.EntityFrameworkCore;
using TaschengeldManager.Core.Entities;
using TaschengeldManager.Core.Interfaces.Repositories;
using TaschengeldManager.Infrastructure.Data;

namespace TaschengeldManager.Infrastructure.Repositories;

public class TotpBackupCodeRepository : Repository<TotpBackupCode, TotpBackupCodeId>, ITotpBackupCodeRepository
{
    public TotpBackupCodeRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<TotpBackupCode>> GetUnusedByUserAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(bc => bc.UserId == userId && !bc.IsUsed)
            .ToListAsync(cancellationToken);
    }

    public async Task<TotpBackupCode?> GetByHashAsync(UserId userId, string codeHash, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(bc => bc.UserId == userId && bc.CodeHash == codeHash && !bc.IsUsed, cancellationToken);
    }

    public async Task DeleteAllByUserAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        await _dbSet
            .Where(bc => bc.UserId == userId)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<int> CountUnusedAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.CountAsync(bc => bc.UserId == userId && !bc.IsUsed, cancellationToken);
    }
}
