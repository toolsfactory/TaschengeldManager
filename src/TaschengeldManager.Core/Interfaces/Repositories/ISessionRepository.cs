using TaschengeldManager.Core.Entities;

namespace TaschengeldManager.Core.Interfaces.Repositories;

/// <summary>
/// Repository interface for Session entities.
/// </summary>
public interface ISessionRepository : IRepository<Session, SessionId>
{
    /// <summary>
    /// Get session by refresh token hash.
    /// </summary>
    Task<Session?> GetByRefreshTokenHashAsync(string refreshTokenHash, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get active sessions for a user.
    /// </summary>
    Task<IReadOnlyList<Session>> GetActiveByUserAsync(UserId userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revoke all sessions for a user except the specified one.
    /// </summary>
    Task RevokeAllExceptAsync(UserId userId, SessionId exceptSessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revoke all sessions for a user.
    /// </summary>
    Task RevokeAllAsync(UserId userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Clean up expired sessions.
    /// </summary>
    Task CleanupExpiredAsync(CancellationToken cancellationToken = default);
}
