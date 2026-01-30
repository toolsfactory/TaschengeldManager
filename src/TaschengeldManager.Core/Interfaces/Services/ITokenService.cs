using TaschengeldManager.Core.Entities;

namespace TaschengeldManager.Core.Interfaces.Services;

/// <summary>
/// Service interface for JWT token generation and validation.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generate access token for a user.
    /// </summary>
    string GenerateAccessToken(User user);

    /// <summary>
    /// Generate refresh token.
    /// </summary>
    string GenerateRefreshToken();

    /// <summary>
    /// Generate temporary MFA token.
    /// </summary>
    string GenerateMfaToken(UserId userId);

    /// <summary>
    /// Validate MFA token and return user ID.
    /// </summary>
    UserId? ValidateMfaToken(string token);

    /// <summary>
    /// Generate secure random token (for invitations, etc.).
    /// </summary>
    string GenerateSecureToken(int length = 32);

    /// <summary>
    /// Hash a token for secure storage.
    /// </summary>
    string HashToken(string token);

    /// <summary>
    /// Verify a token against a hash.
    /// </summary>
    bool VerifyToken(string token, string hash);
}
