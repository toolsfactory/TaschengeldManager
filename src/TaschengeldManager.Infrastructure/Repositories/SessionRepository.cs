using Microsoft.EntityFrameworkCore;
using TaschengeldManager.Core.Entities;
using TaschengeldManager.Core.Interfaces.Repositories;
using TaschengeldManager.Infrastructure.Data;

namespace TaschengeldManager.Infrastructure.Repositories;

public class SessionRepository : Repository<Session, SessionId>, ISessionRepository
{
    public SessionRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Session?> GetByRefreshTokenHashAsync(string refreshTokenHash, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.RefreshTokenHash == refreshTokenHash && !s.IsRevoked, cancellationToken);
    }

    public async Task<IReadOnlyList<Session>> GetActiveByUserAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.UserId == userId && !s.IsRevoked && s.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(s => s.LastActivityAt)
            .ToListAsync(cancellationToken);
    }

    public async Task RevokeAllExceptAsync(UserId userId, SessionId exceptSessionId, CancellationToken cancellationToken = default)
    {
        await _dbSet
            .Where(s => s.UserId == userId && s.Id != exceptSessionId && !s.IsRevoked)
            .ExecuteUpdateAsync(s => s
                .SetProperty(x => x.IsRevoked, true)
                .SetProperty(x => x.RevokedAt, DateTime.UtcNow),
            cancellationToken);
    }

    public async Task RevokeAllAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        await _dbSet
            .Where(s => s.UserId == userId && !s.IsRevoked)
            .ExecuteUpdateAsync(s => s
                .SetProperty(x => x.IsRevoked, true)
                .SetProperty(x => x.RevokedAt, DateTime.UtcNow),
            cancellationToken);
    }

    public async Task CleanupExpiredAsync(CancellationToken cancellationToken = default)
    {
        await _dbSet
            .Where(s => s.ExpiresAt < DateTime.UtcNow || s.IsRevoked)
            .Where(s => s.RevokedAt == null || s.RevokedAt < DateTime.UtcNow.AddDays(-7))
            .ExecuteDeleteAsync(cancellationToken);
    }
}
