using TaschengeldManager.Core.DTOs.Auth;
using TaschengeldManager.Core.Entities;

namespace TaschengeldManager.Core.Interfaces.Services;

/// <summary>
/// Service interface for authentication operations.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Register a new parent user.
    /// </summary>
    Task<RegisterResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Login with email and password.
    /// </summary>
    Task<LoginResult> LoginAsync(LoginRequest request, string? ipAddress, string? userAgent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Login as a child.
    /// </summary>
    Task<LoginResult> ChildLoginAsync(ChildLoginRequest request, string? ipAddress, string? userAgent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verify TOTP code and complete login.
    /// </summary>
    Task<LoginResponse> VerifyTotpAsync(VerifyTotpRequest request, string? ipAddress, string? userAgent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Refresh access token.
    /// </summary>
    Task<LoginResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Logout and invalidate session.
    /// </summary>
    Task LogoutAsync(UserId userId, string refreshToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Logout from all devices.
    /// </summary>
    Task LogoutAllAsync(UserId userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get current user.
    /// </summary>
    Task<UserDto?> GetCurrentUserAsync(UserId userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validate JWT token and return user ID.
    /// </summary>
    Task<UserId?> ValidateTokenAsync(string token, CancellationToken cancellationToken = default);
}
