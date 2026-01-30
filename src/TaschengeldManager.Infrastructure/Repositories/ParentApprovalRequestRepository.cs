using Microsoft.EntityFrameworkCore;
using TaschengeldManager.Core.Entities;
using TaschengeldManager.Core.Enums;
using TaschengeldManager.Core.Interfaces.Repositories;
using TaschengeldManager.Infrastructure.Data;

namespace TaschengeldManager.Infrastructure.Repositories;

public class ParentApprovalRequestRepository : Repository<ParentApprovalRequest, ParentApprovalRequestId>, IParentApprovalRequestRepository
{
    public ParentApprovalRequestRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<ParentApprovalRequest>> GetPendingByFamilyAsync(FamilyId familyId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(par => par.FamilyId == familyId && par.Status == ParentApprovalStatus.Pending && par.ExpiresAt > DateTime.UtcNow)
            .Include(par => par.ChildUser)
            .OrderByDescending(par => par.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<ParentApprovalRequest?> GetPendingByChildAsync(UserId childUserId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(par => par.ChildUserId == childUserId && par.Status == ParentApprovalStatus.Pending && par.ExpiresAt > DateTime.UtcNow, cancellationToken);
    }

    public async Task<int> GetRecentCountByChildAsync(UserId childUserId, TimeSpan window, CancellationToken cancellationToken = default)
    {
        var since = DateTime.UtcNow - window;
        return await _dbSet
            .CountAsync(par => par.ChildUserId == childUserId && par.CreatedAt > since, cancellationToken);
    }

    public async Task ExpireOldRequestsAsync(CancellationToken cancellationToken = default)
    {
        await _dbSet
            .Where(par => par.Status == ParentApprovalStatus.Pending && par.ExpiresAt < DateTime.UtcNow)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.Status, ParentApprovalStatus.Expired), cancellationToken);
    }
}
