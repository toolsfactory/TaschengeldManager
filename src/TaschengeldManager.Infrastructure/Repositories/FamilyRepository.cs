using Microsoft.EntityFrameworkCore;
using TaschengeldManager.Core.Entities;
using TaschengeldManager.Core.Interfaces.Repositories;
using TaschengeldManager.Infrastructure.Data;

namespace TaschengeldManager.Infrastructure.Repositories;

public class FamilyRepository : Repository<Family, FamilyId>, IFamilyRepository
{
    public FamilyRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Family?> GetByFamilyCodeAsync(string familyCode, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(f => f.FamilyCode == familyCode, cancellationToken);
    }

    public async Task<Family?> GetWithMembersAsync(FamilyId familyId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(f => f.Parents)
                .ThenInclude(fp => fp.User)
            .Include(f => f.Relatives)
                .ThenInclude(fr => fr.User)
            .Include(f => f.Children)
                .ThenInclude(c => c.Account)
            .FirstOrDefaultAsync(f => f.Id == familyId, cancellationToken);
    }

    public async Task<IReadOnlyList<Family>> GetFamiliesForParentAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(f => f.Parents.Any(fp => fp.UserId == userId))
            .Include(f => f.Children)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Family>> GetFamiliesForRelativeAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(f => f.Relatives.Any(fr => fr.UserId == userId))
            .Include(f => f.Children)
                .ThenInclude(c => c.Account)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> FamilyCodeExistsAsync(string familyCode, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(f => f.FamilyCode == familyCode, cancellationToken);
    }

    public async Task<bool> IsParentOfFamilyAsync(UserId userId, FamilyId familyId, CancellationToken cancellationToken = default)
    {
        return await _context.FamilyParents.AnyAsync(
            fp => fp.FamilyId == familyId && fp.UserId == userId,
            cancellationToken);
    }

    public async Task<bool> IsRelativeOfFamilyAsync(UserId userId, FamilyId familyId, CancellationToken cancellationToken = default)
    {
        return await _context.FamilyRelatives.AnyAsync(
            fr => fr.FamilyId == familyId && fr.UserId == userId,
            cancellationToken);
    }

    public async Task AddParentAsync(FamilyId familyId, UserId userId, bool isPrimary = false, CancellationToken cancellationToken = default)
    {
        var familyParent = new FamilyParent
        {
            FamilyId = familyId,
            UserId = userId,
            JoinedAt = DateTime.UtcNow,
            IsPrimary = isPrimary
        };
        await _context.FamilyParents.AddAsync(familyParent, cancellationToken);
    }

    public async Task AddRelativeAsync(FamilyId familyId, UserId userId, string? relationshipDescription = null, CancellationToken cancellationToken = default)
    {
        var familyRelative = new FamilyRelative
        {
            FamilyId = familyId,
            UserId = userId,
            JoinedAt = DateTime.UtcNow,
            RelationshipDescription = relationshipDescription
        };
        await _context.FamilyRelatives.AddAsync(familyRelative, cancellationToken);
    }

    public async Task RemoveParentAsync(FamilyId familyId, UserId userId, CancellationToken cancellationToken = default)
    {
        var familyParent = await _context.FamilyParents
            .FirstOrDefaultAsync(fp => fp.FamilyId == familyId && fp.UserId == userId, cancellationToken);
        if (familyParent != null)
        {
            _context.FamilyParents.Remove(familyParent);
        }
    }

    public async Task RemoveRelativeAsync(FamilyId familyId, UserId userId, CancellationToken cancellationToken = default)
    {
        var familyRelative = await _context.FamilyRelatives
            .FirstOrDefaultAsync(fr => fr.FamilyId == familyId && fr.UserId == userId, cancellationToken);
        if (familyRelative != null)
        {
            _context.FamilyRelatives.Remove(familyRelative);
        }
    }
}
