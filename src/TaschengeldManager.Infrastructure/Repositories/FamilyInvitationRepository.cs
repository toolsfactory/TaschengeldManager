using Microsoft.EntityFrameworkCore;
using TaschengeldManager.Core.Entities;
using TaschengeldManager.Core.Enums;
using TaschengeldManager.Core.Interfaces.Repositories;
using TaschengeldManager.Infrastructure.Data;

namespace TaschengeldManager.Infrastructure.Repositories;

public class FamilyInvitationRepository : Repository<FamilyInvitation, FamilyInvitationId>, IFamilyInvitationRepository
{
    public FamilyInvitationRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<FamilyInvitation?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(fi => fi.Family)
            .Include(fi => fi.InvitedByUser)
            .FirstOrDefaultAsync(fi => fi.TokenHash == tokenHash, cancellationToken);
    }

    public async Task<IReadOnlyList<FamilyInvitation>> GetPendingByFamilyAsync(FamilyId familyId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(fi => fi.FamilyId == familyId && fi.Status == InvitationStatus.Pending && fi.ExpiresAt > DateTime.UtcNow)
            .Include(fi => fi.InvitedByUser)
            .OrderByDescending(fi => fi.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<FamilyInvitation>> GetPendingByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.ToUpperInvariant();
        return await _dbSet
            .Where(fi => fi.NormalizedInvitedEmail == normalizedEmail && fi.Status == InvitationStatus.Pending && fi.ExpiresAt > DateTime.UtcNow)
            .Include(fi => fi.Family)
            .Include(fi => fi.InvitedByUser)
            .ToListAsync(cancellationToken);
    }

    public async Task ExpireOldInvitationsAsync(CancellationToken cancellationToken = default)
    {
        await _dbSet
            .Where(fi => fi.Status == InvitationStatus.Pending && fi.ExpiresAt < DateTime.UtcNow)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.Status, InvitationStatus.Expired), cancellationToken);
    }
}
