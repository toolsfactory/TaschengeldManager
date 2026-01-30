using TaschengeldManager.Core.Entities;

namespace TaschengeldManager.Core.Interfaces.Repositories;

/// <summary>
/// Repository interface for LoginAttempt entities.
/// </summary>
public interface ILoginAttemptRepository : IRepository<LoginAttempt, LoginAttemptId>
{
    /// <summary>
    /// Get recent login attempts for a user.
    /// </summary>
    Task<IReadOnlyList<LoginAttempt>> GetRecentByUserAsync(UserId userId, int limit = 10, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get failed attempts in the last time window (for rate limiting).
    /// </summary>
    Task<int> GetFailedCountAsync(string identifier, TimeSpan window, CancellationToken cancellationToken = default);

    /// <summary>
    /// Clean up old attempts.
    /// </summary>
    Task CleanupOldAsync(TimeSpan maxAge, CancellationToken cancellationToken = default);
}
