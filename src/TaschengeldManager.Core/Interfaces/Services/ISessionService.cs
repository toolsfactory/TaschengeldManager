using TaschengeldManager.Core.DTOs.Auth;
using TaschengeldManager.Core.Entities;

namespace TaschengeldManager.Core.Interfaces.Services;

/// <summary>
/// Service interface for session management.
/// </summary>
public interface ISessionService
{
    /// <summary>
    /// Get active sessions for a user.
    /// </summary>
    Task<IReadOnlyList<SessionDto>> GetActiveSessionsAsync(UserId userId, SessionId currentSessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revoke a specific session.
    /// </summary>
    Task RevokeSessionAsync(UserId userId, SessionId sessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revoke all other sessions.
    /// </summary>
    Task RevokeOtherSessionsAsync(UserId userId, SessionId currentSessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get login history.
    /// </summary>
    Task<IReadOnlyList<LoginAttemptDto>> GetLoginHistoryAsync(UserId userId, int limit = 10, CancellationToken cancellationToken = default);
}

/// <summary>
/// Login attempt for display.
/// </summary>
public record LoginAttemptDto
{
    public required DateTime Timestamp { get; init; }
    public required bool Success { get; init; }
    public string? FailureReason { get; init; }
    public string? IpAddress { get; init; }
    public string? Location { get; init; }
    public string? MfaMethod { get; init; }
}
