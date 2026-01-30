using TaschengeldManager.Core.Entities;

namespace TaschengeldManager.Core.Interfaces.Repositories;

/// <summary>
/// Repository interface for BiometricToken entities.
/// </summary>
public interface IBiometricTokenRepository : IRepository<BiometricToken, BiometricTokenId>
{
    /// <summary>
    /// Get token by device ID and user.
    /// </summary>
    Task<BiometricToken?> GetByDeviceAndUserAsync(string deviceId, UserId userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get valid token by device ID.
    /// </summary>
    Task<BiometricToken?> GetValidByDeviceAsync(string deviceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all tokens for a user.
    /// </summary>
    Task<IReadOnlyList<BiometricToken>> GetByUserAsync(UserId userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Invalidate all tokens for a device.
    /// </summary>
    Task InvalidateByDeviceAsync(string deviceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Clean up expired tokens.
    /// </summary>
    Task CleanupExpiredAsync(CancellationToken cancellationToken = default);
}
