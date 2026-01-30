using Microsoft.EntityFrameworkCore;
using TaschengeldManager.Core.Entities;
using TaschengeldManager.Core.Enums;
using TaschengeldManager.Core.Interfaces.Repositories;
using TaschengeldManager.Infrastructure.Data;

namespace TaschengeldManager.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for MoneyRequest entities.
/// </summary>
public class MoneyRequestRepository : Repository<MoneyRequest, MoneyRequestId>, IMoneyRequestRepository
{
    public MoneyRequestRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<MoneyRequest>> GetByChildIdAsync(UserId childUserId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.ChildUser)
            .Include(r => r.RespondedByUser)
            .Where(r => r.ChildUserId == childUserId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MoneyRequest>> GetPendingForFamilyAsync(FamilyId familyId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.ChildUser)
            .Include(r => r.RespondedByUser)
            .Where(r => r.ChildUser!.FamilyId == familyId && r.Status == RequestStatus.Pending)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MoneyRequest>> GetAllForFamilyAsync(FamilyId familyId, RequestStatus? status = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(r => r.ChildUser)
            .Include(r => r.RespondedByUser)
            .Where(r => r.ChildUser!.FamilyId == familyId);

        if (status.HasValue)
        {
            query = query.Where(r => r.Status == status.Value);
        }

        return await query
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<MoneyRequest?> GetWithDetailsAsync(MoneyRequestId requestId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.ChildUser)
            .Include(r => r.RespondedByUser)
            .Include(r => r.ResultingTransaction)
            .FirstOrDefaultAsync(r => r.Id == requestId, cancellationToken);
    }
}
