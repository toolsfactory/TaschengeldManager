using Microsoft.EntityFrameworkCore;
using TaschengeldManager.Core.Entities;
using TaschengeldManager.Core.Interfaces.Repositories;
using TaschengeldManager.Infrastructure.Data;

namespace TaschengeldManager.Infrastructure.Repositories;

public class UserRepository : Repository<User, UserId>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.ToUpperInvariant();
        return await _dbSet.FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken);
    }

    public async Task<User?> GetByNicknameInFamilyAsync(FamilyId familyId, string nickname, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(
            u => u.FamilyId == familyId && u.Nickname.ToLower() == nickname.ToLower(),
            cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.ToUpperInvariant();
        return await _dbSet.AnyAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken);
    }

    public async Task<User?> GetWithPasskeysAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(u => u.Passkeys)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }

    public async Task<User?> GetWithSessionsAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(u => u.Sessions.Where(s => !s.IsRevoked && s.ExpiresAt > DateTime.UtcNow))
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }

    public async Task<IReadOnlyList<User>> GetChildrenByFamilyAsync(FamilyId familyId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(u => u.FamilyId == familyId)
            .Include(u => u.Account)
            .ToListAsync(cancellationToken);
    }
}
