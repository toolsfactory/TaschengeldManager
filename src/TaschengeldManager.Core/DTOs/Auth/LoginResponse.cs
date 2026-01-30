namespace TaschengeldManager.Core.DTOs.Auth;

/// <summary>
/// Response after successful login.
/// </summary>
public record LoginResponse
{
    /// <summary>
    /// JWT access token.
    /// </summary>
    public required string AccessToken { get; init; }

    /// <summary>
    /// Refresh token for obtaining new access tokens.
    /// </summary>
    public required string RefreshToken { get; init; }

    /// <summary>
    /// When the access token expires.
    /// </summary>
    public required DateTime ExpiresAt { get; init; }

    /// <summary>
    /// User information.
    /// </summary>
    public required UserDto User { get; init; }
}
