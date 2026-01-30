using Microsoft.EntityFrameworkCore;
using TaschengeldManager.Core.Entities;
using TaschengeldManager.Core.Interfaces.Repositories;
using TaschengeldManager.Infrastructure.Data;

namespace TaschengeldManager.Infrastructure.Repositories;

public class BiometricTokenRepository : Repository<BiometricToken, BiometricTokenId>, IBiometricTokenRepository
{
    public BiometricTokenRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<BiometricToken?> GetByDeviceAndUserAsync(string deviceId, UserId userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(bt => bt.DeviceId == deviceId && bt.UserId == userId, cancellationToken);
    }

    public async Task<BiometricToken?> GetValidByDeviceAsync(string deviceId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(bt => bt.User)
            .FirstOrDefaultAsync(bt => bt.DeviceId == deviceId && bt.IsValid && bt.ExpiresAt > DateTime.UtcNow, cancellationToken);
    }

    public async Task<IReadOnlyList<BiometricToken>> GetByUserAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(bt => bt.UserId == userId)
            .OrderByDescending(bt => bt.LastUsedAt ?? bt.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task InvalidateByDeviceAsync(string deviceId, CancellationToken cancellationToken = default)
    {
        await _dbSet
            .Where(bt => bt.DeviceId == deviceId)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.IsValid, false), cancellationToken);
    }

    public async Task CleanupExpiredAsync(CancellationToken cancellationToken = default)
    {
        await _dbSet
            .Where(bt => bt.ExpiresAt < DateTime.UtcNow || !bt.IsValid)
            .ExecuteDeleteAsync(cancellationToken);
    }
}
