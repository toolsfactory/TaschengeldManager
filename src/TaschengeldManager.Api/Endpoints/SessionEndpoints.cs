using System.Security.Claims;
using TaschengeldManager.Api.Extensions;
using TaschengeldManager.Core.DTOs.Auth;
using TaschengeldManager.Core.Entities;
using TaschengeldManager.Core.Interfaces.Services;

namespace TaschengeldManager.Api.Endpoints;

/// <summary>
/// Session management endpoints.
/// </summary>
public static class SessionEndpoints
{
    public static IEndpointRouteBuilder MapSessionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/session")
            .WithTags("Session")
            
            .RequireAuthorization();

        group.MapGet("/", GetActiveSessions)
            .WithSummary("Get active sessions for the current user")
            .Produces<IReadOnlyList<SessionDto>>(200);

        group.MapDelete("/{sessionId:guid}", RevokeSession)
            .WithSummary("Revoke a specific session")
            .Produces(204)
            .Produces(400);

        group.MapDelete("/others", RevokeOtherSessions)
            .WithSummary("Revoke all other sessions")
            .Produces(204);

        group.MapGet("/history", GetLoginHistory)
            .WithSummary("Get login history")
            .Produces<IReadOnlyList<LoginAttemptDto>>(200);

        return app;
    }

    private static async Task<IResult> GetActiveSessions(
        ClaimsPrincipal user,
        ISessionService sessionService,
        CancellationToken cancellationToken)
    {
        var userId = user.GetUserId();
        var sessionId = user.GetSessionId();
        var sessions = await sessionService.GetActiveSessionsAsync(userId, sessionId, cancellationToken);
        return Results.Ok(sessions);
    }

    private static async Task<IResult> RevokeSession(
        Guid sessionId,
        ClaimsPrincipal user,
        ISessionService sessionService,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = user.GetUserId();
            await sessionService.RevokeSessionAsync(userId, new SessionId(sessionId), cancellationToken);
            return Results.NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }

    private static async Task<IResult> RevokeOtherSessions(
        ClaimsPrincipal user,
        ISessionService sessionService,
        CancellationToken cancellationToken)
    {
        var userId = user.GetUserId();
        var sessionId = user.GetSessionId();
        await sessionService.RevokeOtherSessionsAsync(userId, sessionId, cancellationToken);
        return Results.NoContent();
    }

    private static async Task<IResult> GetLoginHistory(
        ClaimsPrincipal user,
        ISessionService sessionService,
        int limit = 10,
        CancellationToken cancellationToken = default)
    {
        var userId = user.GetUserId();
        var history = await sessionService.GetLoginHistoryAsync(userId, limit, cancellationToken);
        return Results.Ok(history);
    }
}
