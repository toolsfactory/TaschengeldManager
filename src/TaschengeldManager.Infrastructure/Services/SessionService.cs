using Microsoft.Extensions.Logging;
using TaschengeldManager.Core.DTOs.Auth;
using TaschengeldManager.Core.Entities;
using TaschengeldManager.Core.Interfaces;
using TaschengeldManager.Core.Interfaces.Services;

namespace TaschengeldManager.Infrastructure.Services;

public class SessionService : ISessionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SessionService> _logger;

    public SessionService(
        IUnitOfWork unitOfWork,
        ILogger<SessionService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<IReadOnlyList<SessionDto>> GetActiveSessionsAsync(UserId userId, SessionId currentSessionId, CancellationToken cancellationToken = default)
    {
        var sessions = await _unitOfWork.Sessions.GetActiveByUserAsync(userId, cancellationToken);

        return sessions.Select(s => new SessionDto
        {
            Id = s.Id.Value,
            DeviceInfo = s.DeviceInfo,
            IpAddress = s.IpAddress,
            CreatedAt = s.CreatedAt,
            LastActivityAt = s.LastActivityAt,
            IsCurrent = s.Id == currentSessionId,
            IsTrustedDevice = s.IsTrustedDevice
        }).ToList();
    }

    public async Task RevokeSessionAsync(UserId userId, SessionId sessionId, CancellationToken cancellationToken = default)
    {
        var session = await _unitOfWork.Sessions.GetByIdAsync(sessionId, cancellationToken);

        if (session == null || session.UserId != userId)
        {
            throw new InvalidOperationException("Session not found");
        }

        session.IsRevoked = true;
        session.RevokedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Session {SessionId} revoked by user {UserId}", sessionId.Value, userId.Value);
    }

    public async Task RevokeOtherSessionsAsync(UserId userId, SessionId currentSessionId, CancellationToken cancellationToken = default)
    {
        var sessions = await _unitOfWork.Sessions.GetActiveByUserAsync(userId, cancellationToken);

        foreach (var session in sessions.Where(s => s.Id != currentSessionId))
        {
            session.IsRevoked = true;
            session.RevokedAt = DateTime.UtcNow;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("All other sessions revoked for user {UserId}", userId.Value);
    }

    public async Task<IReadOnlyList<LoginAttemptDto>> GetLoginHistoryAsync(UserId userId, int limit = 10, CancellationToken cancellationToken = default)
    {
        var attempts = await _unitOfWork.LoginAttempts.GetRecentByUserAsync(userId, limit, cancellationToken);

        return attempts.Select(a => new LoginAttemptDto
        {
            Timestamp = a.CreatedAt,
            Success = a.Success,
            FailureReason = a.FailureReason,
            IpAddress = a.IpAddress,
            Location = a.Location,
            MfaMethod = a.MfaMethod
        }).ToList();
    }
}
