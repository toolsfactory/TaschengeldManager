using Microsoft.EntityFrameworkCore;
using TaschengeldManager.Core.Entities;
using TaschengeldManager.Core.Interfaces.Repositories;
using TaschengeldManager.Infrastructure.Data;

namespace TaschengeldManager.Infrastructure.Repositories;

public class PasskeyRepository : Repository<Passkey, PasskeyId>, IPasskeyRepository
{
    public PasskeyRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Passkey?> GetByCredentialIdAsync(string credentialId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.CredentialId == credentialId, cancellationToken);
    }

    public async Task<IReadOnlyList<Passkey>> GetByUserAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.LastUsedAt ?? p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> CredentialIdExistsAsync(string credentialId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(p => p.CredentialId == credentialId, cancellationToken);
    }
}
