using TaschengeldManager.Core.DTOs.Auth;

namespace TaschengeldManager.Mobile.Services.Auth;

/// <summary>
/// Service for handling authentication, token management, and biometric login
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Gets the currently logged-in user, or null if not authenticated
    /// </summary>
    UserDto? CurrentUser { get; }

    /// <summary>
    /// Gets whether the user is currently authenticated
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Gets whether biometric login is available and enabled
    /// </summary>
    bool IsBiometricEnabled { get; }

    /// <summary>
    /// Event fired when authentication state changes
    /// </summary>
    event EventHandler<AuthenticationStateChangedEventArgs>? AuthenticationStateChanged;

    /// <summary>
    /// Register a new parent account
    /// </summary>
    Task<RegisterResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default);

    /// <summary>
    /// Login with email and password (for parents)
    /// </summary>
    Task<LoginResult> LoginAsync(LoginRequest request, CancellationToken ct = default);

    /// <summary>
    /// Login with family code, nickname and PIN (for children)
    /// </summary>
    Task<LoginResult> LoginChildAsync(ChildLoginRequest request, CancellationToken ct = default);

    /// <summary>
    /// Login with biometric authentication
    /// </summary>
    Task<LoginResult> LoginWithBiometricAsync(CancellationToken ct = default);

    /// <summary>
    /// Verify TOTP code during MFA flow
    /// </summary>
    Task<LoginResponse> VerifyTotpAsync(string mfaToken, string code, CancellationToken ct = default);

    /// <summary>
    /// Logout and clear all tokens
    /// </summary>
    Task LogoutAsync(CancellationToken ct = default);

    /// <summary>
    /// Enable biometric login for the current device
    /// </summary>
    Task<bool> EnableBiometricAsync(CancellationToken ct = default);

    /// <summary>
    /// Disable biometric login for the current device
    /// </summary>
    Task DisableBiometricAsync(CancellationToken ct = default);

    /// <summary>
    /// Try to restore the session from stored tokens
    /// </summary>
    Task<bool> TryRestoreSessionAsync(CancellationToken ct = default);

    /// <summary>
    /// Get the current access token for API calls
    /// </summary>
    Task<string?> GetAccessTokenAsync(CancellationToken ct = default);
}

/// <summary>
/// Result of a login attempt
/// </summary>
public record LoginResult
{
    public bool Success { get; init; }
    public bool RequiresMfa { get; init; }
    public string? MfaToken { get; init; }
    public string[]? AvailableMfaMethods { get; init; }
    public string? ErrorMessage { get; init; }
    public UserDto? User { get; init; }

    public static LoginResult Succeeded(UserDto user) => new() { Success = true, User = user };
    public static LoginResult MfaRequired(string mfaToken, string[] methods) => new() { RequiresMfa = true, MfaToken = mfaToken, AvailableMfaMethods = methods };
    public static LoginResult Failed(string error) => new() { ErrorMessage = error };
}

/// <summary>
/// Event args for authentication state changes
/// </summary>
public class AuthenticationStateChangedEventArgs : EventArgs
{
    public bool IsAuthenticated { get; init; }
    public UserDto? User { get; init; }
}
