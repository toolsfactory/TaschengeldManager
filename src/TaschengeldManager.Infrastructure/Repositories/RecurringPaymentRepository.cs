using Microsoft.EntityFrameworkCore;
using TaschengeldManager.Core.Entities;
using TaschengeldManager.Core.Interfaces.Repositories;
using TaschengeldManager.Infrastructure.Data;

namespace TaschengeldManager.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for RecurringPayment entities.
/// </summary>
public class RecurringPaymentRepository : Repository<RecurringPayment, RecurringPaymentId>, IRecurringPaymentRepository
{
    public RecurringPaymentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<RecurringPayment>> GetByAccountIdAsync(AccountId accountId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Account)
                .ThenInclude(a => a!.User)
            .Include(p => p.CreatedByUser)
            .Where(p => p.AccountId == accountId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<RecurringPayment>> GetByCreatorIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Account)
                .ThenInclude(a => a!.User)
            .Include(p => p.CreatedByUser)
            .Where(p => p.CreatedByUserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<RecurringPayment>> GetDuePaymentsAsync(DateTime upToDate, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Account)
                .ThenInclude(a => a!.User)
            .Include(p => p.CreatedByUser)
            .Where(p => p.IsActive && p.NextExecutionDate <= upToDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<RecurringPayment?> GetWithDetailsAsync(RecurringPaymentId paymentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Account)
                .ThenInclude(a => a!.User)
            .Include(p => p.CreatedByUser)
            .FirstOrDefaultAsync(p => p.Id == paymentId, cancellationToken);
    }
}
