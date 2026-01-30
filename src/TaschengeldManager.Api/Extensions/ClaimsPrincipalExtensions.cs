using System.Security.Claims;
using TaschengeldManager.Core.Entities;

namespace TaschengeldManager.Api.Extensions;

/// <summary>
/// Extension methods for ClaimsPrincipal to extract user information from JWT claims.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Gets the user ID from the JWT claims.
    /// </summary>
    /// <exception cref="UnauthorizedAccessException">Thrown if the user ID claim is missing or invalid.</exception>
    public static UserId GetUserId(this ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid user ID in token");
        }
        return new UserId(userId);
    }

    /// <summary>
    /// Gets the session ID from the JWT claims, or a default SessionId if not found.
    /// </summary>
    public static SessionId GetSessionId(this ClaimsPrincipal user)
    {
        var sessionIdClaim = user.FindFirst("session_id")?.Value;
        if (!string.IsNullOrEmpty(sessionIdClaim) && Guid.TryParse(sessionIdClaim, out var sessionId))
        {
            return new SessionId(sessionId);
        }
        return new SessionId(Guid.Empty);
    }
}
