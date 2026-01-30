using Microsoft.EntityFrameworkCore;
using TaschengeldManager.Core.Entities;
using TaschengeldManager.Core.Interfaces.Repositories;
using TaschengeldManager.Infrastructure.Data;

namespace TaschengeldManager.Infrastructure.Repositories;

public class LoginAttemptRepository : Repository<LoginAttempt, LoginAttemptId>, ILoginAttemptRepository
{
    public LoginAttemptRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<LoginAttempt>> GetRecentByUserAsync(UserId userId, int limit = 10, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(la => la.UserId == userId)
            .OrderByDescending(la => la.CreatedAt)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetFailedCountAsync(string identifier, TimeSpan window, CancellationToken cancellationToken = default)
    {
        var since = DateTime.UtcNow - window;
        return await _dbSet
            .CountAsync(la => la.Identifier == identifier && !la.Success && la.CreatedAt > since, cancellationToken);
    }

    public async Task CleanupOldAsync(TimeSpan maxAge, CancellationToken cancellationToken = default)
    {
        var cutoff = DateTime.UtcNow - maxAge;
        await _dbSet
            .Where(la => la.CreatedAt < cutoff)
            .ExecuteDeleteAsync(cancellationToken);
    }
}
