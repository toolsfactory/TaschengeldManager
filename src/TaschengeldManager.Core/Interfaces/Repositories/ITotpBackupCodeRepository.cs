using TaschengeldManager.Core.Entities;

namespace TaschengeldManager.Core.Interfaces.Repositories;

/// <summary>
/// Repository interface for TotpBackupCode entities.
/// </summary>
public interface ITotpBackupCodeRepository : IRepository<TotpBackupCode, TotpBackupCodeId>
{
    /// <summary>
    /// Get unused backup codes for a user.
    /// </summary>
    Task<IReadOnlyList<TotpBackupCode>> GetUnusedByUserAsync(UserId userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get backup code by hash for a user.
    /// </summary>
    Task<TotpBackupCode?> GetByHashAsync(UserId userId, string codeHash, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete all backup codes for a user.
    /// </summary>
    Task DeleteAllByUserAsync(UserId userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Count unused backup codes for a user.
    /// </summary>
    Task<int> CountUnusedAsync(UserId userId, CancellationToken cancellationToken = default);
}
